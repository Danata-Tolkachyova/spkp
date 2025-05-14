#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/ipc.h>
#include <sys/msg.h>
#include <sys/types.h>
#include <time.h>
#include <signal.h>
#include <unistd.h>
#include <errno.h>
#include <dirent.h>

#define MSGQ_KEY 1234
#define BUFFER_SIZE 1024
#define STORAGE_DIR "chat_storage"

struct msg_buffer {
    long msg_type;
    char msg_text[BUFFER_SIZE];
};

int msgq_id;
FILE *current_log_file = NULL;
char current_date[11] = {0}; // YYYY-MM-DD

void handle_signal(int sig) {
    printf("\nЗавершение сервера-хранилища...\n");
    
    if (current_log_file) {
        fclose(current_log_file);
    }
    
    if (msgctl(msgq_id, IPC_RMID, NULL) == -1 && errno != EINVAL) {
        perror("msgctl");
    }
    
    exit(0);
}

void ensure_storage_dir() {
    if (access(STORAGE_DIR, F_OK)) {
        if (mkdir(STORAGE_DIR, 0777)) {
            perror("mkdir");
            exit(EXIT_FAILURE);
        }
    }
}

void get_current_date_str(char *date_str) {
    time_t now = time(NULL);
    struct tm *tm_info = localtime(&now);
    strftime(date_str, 11, "%Y-%m-%d", tm_info);
}

FILE* get_log_file() {
    char date_str[11];
    get_current_date_str(date_str);
    
    if (current_log_file && strcmp(current_date, date_str)) {
        fclose(current_log_file);
        current_log_file = NULL;
    }
    
    if (!current_log_file) {
        char filename[256];
        snprintf(filename, sizeof(filename), STORAGE_DIR "/chat_%s.log", date_str);
        
        current_log_file = fopen(filename, "a");
        if (!current_log_file) {
            perror("fopen");
            exit(EXIT_FAILURE);
        }
        
        strncpy(current_date, date_str, sizeof(current_date));
    }
    
    return current_log_file;
}

void save_message(const char *message) {
    ensure_storage_dir();
    
    FILE *log_file = get_log_file();
    if (!log_file) return;
    
    time_t now = time(NULL);
    char time_str[20];
    struct tm *tm_info = localtime(&now);
    strftime(time_str, sizeof(time_str), "%H:%M:%S", tm_info);
    
    fprintf(log_file, "[%s] %s\n", time_str, message);
    fflush(log_file);
    
    printf("Сообщение сохранено: %s\n", message);
}

int main() {
    signal(SIGINT, handle_signal);
    signal(SIGTERM, handle_signal);
    
    ensure_storage_dir();
    
    msgq_id = msgget(MSGQ_KEY, IPC_CREAT | 0666);
    if (msgq_id == -1) {
        perror("msgget");
        exit(EXIT_FAILURE);
    }
    
    printf("Сервер-хранилище запущен. Ожидание сообщений...\n");
    
    struct msg_buffer msg;
    
    while (1) {
        ssize_t msg_size = msgrcv(msgq_id, &msg, sizeof(msg.msg_text), 1, 0);
        if (msg_size == -1) {
            if (errno == EINTR) continue;
            perror("msgrcv");
            break;
        }
        
        save_message(msg.msg_text);
    }
    
    if (current_log_file) {
        fclose(current_log_file);
    }
    
    return 0;
}
