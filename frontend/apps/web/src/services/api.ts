import { Task, CreateTaskDto, UpdateTaskDto } from '../hooks/useTasks';

interface User {
  id: string;
  email: string;
  name: string;
  avatar?: string;
}

interface LoginResponse {
  token: string;
  user: User;
}

interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}

class ApiService {
  private baseUrl: string;
  private token: string | null = null;

  constructor() {
    this.baseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';
    this.token = localStorage.getItem('authToken');
  }

  private getHeaders(): HeadersInit {
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
    };

    if (this.token) {
      headers['Authorization'] = `Bearer ${this.token}`;
    }

    return headers;
  }

  private async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      let errorMessage = 'An error occurred';
      try {
        const errorData: ApiError = await response.json();
        errorMessage = errorData.message || errorMessage;
      } catch {
        // If parsing fails, use status text
        errorMessage = response.statusText || errorMessage;
      }
      throw new Error(errorMessage);
    }

    return response.json();
  }

  setToken(token: string) {
    this.token = token;
    localStorage.setItem('authToken', token);
  }

  clearToken() {
    this.token = null;
    localStorage.removeItem('authToken');
  }

  // Authentication endpoints
  async login(email: string, password: string): Promise<LoginResponse> {
    const response = await fetch(`${this.baseUrl}/auth/login`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify({ email, password }),
    });

    const result = await this.handleResponse<LoginResponse>(response);
    this.setToken(result.token);
    return result;
  }

  async register(email: string, password: string, name: string): Promise<LoginResponse> {
    const response = await fetch(`${this.baseUrl}/auth/register`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify({ email, password, name }),
    });

    const result = await this.handleResponse<LoginResponse>(response);
    this.setToken(result.token);
    return result;
  }

  async loginWithGoogle(): Promise<LoginResponse> {
    // For now, redirect to Google OAuth endpoint
    // This should be implemented based on your backend's Google OAuth flow
    window.location.href = `${this.baseUrl}/auth/google-login`;
    return Promise.reject(new Error('Redirecting to Google OAuth...'));
  }

  async validateToken(): Promise<User> {
    const response = await fetch(`${this.baseUrl}/auth/me`, {
      headers: this.getHeaders(),
    });

    return this.handleResponse<User>(response);
  }

  // Task endpoints
  async getTasks(): Promise<Task[]> {
    const response = await fetch(`${this.baseUrl}/tasks`, {
      headers: this.getHeaders(),
    });

    return this.handleResponse<Task[]>(response);
  }

  async getTaskById(id: string): Promise<Task> {
    const response = await fetch(`${this.baseUrl}/tasks/${id}`, {
      headers: this.getHeaders(),
    });

    return this.handleResponse<Task>(response);
  }

  async createTask(task: CreateTaskDto): Promise<Task> {
    const response = await fetch(`${this.baseUrl}/tasks`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify({
        ...task,
        dueDate: task.dueDate?.toISOString(),
        reminder: task.reminder?.toISOString(),
      }),
    });

    return this.handleResponse<Task>(response);
  }

  async updateTask(task: UpdateTaskDto): Promise<Task> {
    const response = await fetch(`${this.baseUrl}/tasks/${task.id}`, {
      method: 'PUT',
      headers: this.getHeaders(),
      body: JSON.stringify({
        ...task,
        dueDate: task.dueDate?.toISOString(),
        reminder: task.reminder?.toISOString(),
      }),
    });

    return this.handleResponse<Task>(response);
  }

  async deleteTask(id: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/tasks/${id}`, {
      method: 'DELETE',
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to delete task');
    }
  }

  async completeTask(id: string): Promise<Task> {
    const response = await fetch(`${this.baseUrl}/tasks/${id}/complete`, {
      method: 'PATCH',
      headers: this.getHeaders(),
    });

    return this.handleResponse<Task>(response);
  }

  // Sub-task endpoints
  async addSubTask(taskId: string, title: string): Promise<{ id: string; title: string; completed: boolean; taskId: string }> {
    const response = await fetch(`${this.baseUrl}/tasks/${taskId}/subtasks`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify({ title }),
    });

    return this.handleResponse<{ id: string; title: string; completed: boolean; taskId: string }>(response);
  }

  async toggleSubTask(taskId: string, subTaskId: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/tasks/${taskId}/subtasks/${subTaskId}/toggle`, {
      method: 'PATCH',
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to toggle sub-task');
    }
  }

  async deleteSubTask(taskId: string, subTaskId: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/tasks/${taskId}/subtasks/${subTaskId}`, {
      method: 'DELETE',
      headers: this.getHeaders(),
    });

    if (!response.ok) {
      throw new Error('Failed to delete sub-task');
    }
  }
}

export const apiService = new ApiService();
