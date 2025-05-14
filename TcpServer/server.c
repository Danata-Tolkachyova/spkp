#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <sys/ipc.h>
#include <sys/msg.h>
#include <signal.h>
#include <time.h>
#include <errno.h>
#include <arpa/inet.h>

#define PORT 8888
#define MAX_CLIENTS 10
#define BUFFER_SIZE 1024
#define MSGQ_KEY 1234

struct msg_buffer {
    long msg_type;
    char msg_text[BUFFER_SIZE];
};

int msgq_id;
int client_sockets[MAX_CLIENTS] = {0};

void handle_signal(int sig) {
    printf("\nЗавершение сервера...\n");
    
    for (int i = 0; i < MAX_CLIENTS; i++) {
        if (client_sockets[i] > 0) {
            close(client_sockets[i]);
        }
    }
    
    if (msgctl(msgq_id, IPC_RMID, NULL) == -1) {
        perror("msgctl");
    }
    
    exit(0);
}

void send_to_storage(const char *message) {
    struct msg_buffer msg;
    msg.msg_type = 1;
    strncpy(msg.msg_text, message, BUFFER_SIZE - 1);
    msg.msg_text[BUFFER_SIZE - 1] = '\0';
    
    if (msgsnd(msgq_id, &msg, sizeof(msg.msg_text), 0) == -1) {
        perror("msgsnd");
    }
}

void broadcast_message(const char *message, int exclude_fd) {
    for (int i = 0; i < MAX_CLIENTS; i++) {
        if (client_sockets[i] > 0 && client_sockets[i] != exclude_fd) {
            if (send(client_sockets[i], message, strlen(message), 0) == -1) {
                perror("send");
                close(client_sockets[i]);
                client_sockets[i] = 0;
            }
        }
    }
}

int main() {
    int server_fd, new_socket, activity, i, valread, sd;
    int max_sd;
    struct sockaddr_in address;
    socklen_t addrlen = sizeof(address);
    char buffer[BUFFER_SIZE] = {0};
    fd_set readfds;
    
    signal(SIGINT, handle_signal);
    signal(SIGTERM, handle_signal);
    
    // Инициализация очереди сообщений
    if ((msgq_id = msgget(MSGQ_KEY, IPC_CREAT | 0666)) == -1) {
        perror("msgget");
        exit(EXIT_FAILURE);
    }
    
    // Создание TCP сокета
    if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == 0) {
        perror("socket failed");
        exit(EXIT_FAILURE);
    }
    
    // Настройка параметров сокета
    int opt = 1;
    if (setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR, (char *)&opt, sizeof(opt)) < 0) {
        perror("setsockopt");
        exit(EXIT_FAILURE);
    }
    
    // Инициализация структуры адреса
    memset(&address, 0, sizeof(address));
    address.sin_family = AF_INET;
    address.sin_addr.s_addr = INADDR_ANY;
    address.sin_port = htons(PORT);
    
    // Привязка сокета
    if (bind(server_fd, (struct sockaddr *)&address, sizeof(address)) < 0) {
        perror("bind failed");
        exit(EXIT_FAILURE);
    }
    
    if (listen(server_fd, 3) < 0) {
        perror("listen");
        exit(EXIT_FAILURE);
    }
    
    printf("Сервер чата запущен на порту %d\n", PORT);
    
    while (1) {
        FD_ZERO(&readfds);
        FD_SET(server_fd, &readfds);
        max_sd = server_fd;
        
        for (i = 0; i < MAX_CLIENTS; i++) {
            sd = client_sockets[i];
            if (sd > 0) {
                FD_SET(sd, &readfds);
            }
            if (sd > max_sd) {
                max_sd = sd;
            }
        }
        
        activity = select(max_sd + 1, &readfds, NULL, NULL, NULL);
        if ((activity < 0) && (errno != EINTR)) {
            perror("select error");
        }
        
        if (FD_ISSET(server_fd, &readfds)) {
            if ((new_socket = accept(server_fd, (struct sockaddr *)&address, &addrlen)) < 0) {
                perror("accept");
                continue;
            }
            
            printf("Новое подключение, socket fd: %d, IP: %s, порт: %d\n",
                   new_socket, inet_ntoa(address.sin_addr), ntohs(address.sin_port));
            
            for (i = 0; i < MAX_CLIENTS; i++) {
                if (client_sockets[i] == 0) {
                    client_sockets[i] = new_socket;
                    printf("Добавлен в список сокетов как %d\n", i);
                    
                    char welcome[100];
                    snprintf(welcome, sizeof(welcome), "Добро пожаловать в чат! Ваш ID: %d\n", new_socket);
                    if (send(new_socket, welcome, strlen(welcome), 0) == -1) {
                        perror("send");
                    }
                    break;
                }
            }
        }
        
        for (i = 0; i < MAX_CLIENTS; i++) {
            sd = client_sockets[i];
            if (sd > 0 && FD_ISSET(sd, &readfds)) {
                if ((valread = read(sd, buffer, BUFFER_SIZE - 1)) == 0) {
                    getpeername(sd, (struct sockaddr*)&address, &addrlen);
                    printf("Клиент отключился, IP %s, порт %d\n",
                           inet_ntoa(address.sin_addr), ntohs(address.sin_port));
                    
                    close(sd);
                    client_sockets[i] = 0;
                } else {
                    buffer[valread] = '\0';
                    
                    time_t now = time(NULL);
                    struct tm *tm_info = localtime(&now);
                    char time_str[20];
                    strftime(time_str, sizeof(time_str), "%H:%M:%S", tm_info);
                    
                    char formatted_msg[BUFFER_SIZE + 50];
                    snprintf(formatted_msg, sizeof(formatted_msg), "[%s] Клиент %d: %s", 
                            time_str, sd, buffer);
                    
                    printf("%s\n", formatted_msg);
                    
                    send_to_storage(formatted_msg);
                    broadcast_message(formatted_msg, sd);
                }
            }
        }
    }
    
    return 0;
}
