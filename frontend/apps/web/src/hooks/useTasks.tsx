import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { apiService } from '../services/api';

export interface Task {
  id: string;
  title: string;
  description?: string;
  status: 'pending' | 'in-progress' | 'completed';
  priority: 'low' | 'medium' | 'high';
  dueDate?: Date;
  reminder?: Date;
  repeatPattern?: string;
  groupId?: string;
  subTasks?: SubTask[];
  createdAt: Date;
  updatedAt: Date;
}

export interface SubTask {
  id: string;
  title: string;
  completed: boolean;
  taskId: string;
}

export interface CreateTaskDto {
  title: string;
  description?: string;
  priority?: 'low' | 'medium' | 'high';
  dueDate?: Date;
  reminder?: Date;
  repeatPattern?: string;
  groupId?: string;
}

export interface UpdateTaskDto {
  id: string;
  title?: string;
  description?: string;
  status?: 'pending' | 'in-progress' | 'completed';
  priority?: 'low' | 'medium' | 'high';
  dueDate?: Date;
  reminder?: Date;
  repeatPattern?: string;
  groupId?: string;
}

interface TasksContextType {
  tasks: Task[];
  isLoading: boolean;
  error: string | null;
  createTask: (task: CreateTaskDto) => Promise<Task>;
  updateTask: (task: UpdateTaskDto) => Promise<Task>;
  deleteTask: (id: string) => Promise<void>;
  completeTask: (id: string) => Promise<Task>;
  addSubTask: (taskId: string, title: string) => Promise<SubTask>;
  toggleSubTask: (taskId: string, subTaskId: string) => Promise<void>;
  deleteSubTask: (taskId: string, subTaskId: string) => Promise<void>;
  getTaskById: (id: string) => Task | undefined;
  getTasksByStatus: (status: Task['status']) => Task[];
  getTasksByPriority: (priority: Task['priority']) => Task[];
  getOverdueTasks: () => Task[];
  getTasksDueToday: () => Task[];
}

const TasksContext = createContext<TasksContextType | undefined>(undefined);

export const useTasks = () => {
  const context = useContext(TasksContext);
  if (context === undefined) {
    throw new Error('useTasks must be used within a TaskProvider');
  }
  return context;
};

interface TaskProviderProps {
  children: ReactNode;
}

export const TaskProvider: React.FC<TaskProviderProps> = ({ children }) => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadTasks();
  }, []);

  const loadTasks = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const tasks = await apiService.getTasks();
      setTasks(tasks);
    } catch (err) {
      setError('Failed to load tasks');
      console.error('Error loading tasks:', err);
      
      // Fallback to mock data during development
      const mockTasks: Task[] = [
        {
          id: '1',
          title: 'Complete project documentation',
          description: 'Write comprehensive documentation for the Todo app project',
          status: 'pending',
          priority: 'high',
          dueDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000), // 2 days from now
          createdAt: new Date(),
          updatedAt: new Date(),
        },
        {
          id: '2',
          title: 'Review code changes',
          description: 'Review pull requests and provide feedback',
          status: 'in-progress',
          priority: 'medium',
          dueDate: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000), // 1 day from now
          createdAt: new Date(),
          updatedAt: new Date(),
        },
        {
          id: '3',
          title: 'Setup development environment',
          description: 'Configure local development environment',
          status: 'completed',
          priority: 'low',
          createdAt: new Date(),
          updatedAt: new Date(),
        },
      ];
      
      setTasks(mockTasks);
    } finally {
      setIsLoading(false);
    }
  };

  const createTask = async (taskData: CreateTaskDto): Promise<Task> => {
    setIsLoading(true);
    setError(null);
    try {
      const newTask = await apiService.createTask(taskData);
      setTasks(prev => [...prev, newTask]);
      return newTask;
    } catch (err) {
      setError('Failed to create task');
      console.error('Error creating task:', err);
      
      // Fallback to mock implementation for development
      const newTask: Task = {
        id: Date.now().toString(),
        title: taskData.title,
        description: taskData.description,
        status: 'pending',
        priority: taskData.priority || 'medium',
        dueDate: taskData.dueDate,
        reminder: taskData.reminder,
        repeatPattern: taskData.repeatPattern,
        groupId: taskData.groupId,
        subTasks: [],
        createdAt: new Date(),
        updatedAt: new Date(),
      };
      
      setTasks(prev => [...prev, newTask]);
      return newTask;
    } finally {
      setIsLoading(false);
    }
  };

  const updateTask = async (taskData: UpdateTaskDto): Promise<Task> => {
    setIsLoading(true);
    setError(null);
    try {
      // TODO: Implement API call to update task
      // const response = await api.put(`/tasks/${taskData.id}`, taskData);
      // const updatedTask = response.data;
      // setTasks(prev => prev.map(task => task.id === taskData.id ? updatedTask : task));
      // return updatedTask;
      
      // Mock implementation
      setTasks(prev => prev.map(task => {
        if (task.id === taskData.id) {
          return {
            ...task,
            ...taskData,
            updatedAt: new Date(),
          };
        }
        return task;
      }));
      
      const updatedTask = tasks.find(task => task.id === taskData.id);
      if (!updatedTask) {
        throw new Error('Task not found');
      }
      
      return updatedTask;
    } catch (err) {
      setError('Failed to update task');
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const deleteTask = async (id: string): Promise<void> => {
    setIsLoading(true);
    setError(null);
    try {
      // TODO: Implement API call to delete task
      // await api.delete(`/tasks/${id}`);
      // setTasks(prev => prev.filter(task => task.id !== id));
      
      // Mock implementation
      setTasks(prev => prev.filter(task => task.id !== id));
    } catch (err) {
      setError('Failed to delete task');
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const completeTask = async (id: string): Promise<Task> => {
    return updateTask({ id, status: 'completed' });
  };

  const addSubTask = async (taskId: string, title: string): Promise<SubTask> => {
    setIsLoading(true);
    setError(null);
    try {
      const newSubTask: SubTask = {
        id: `${taskId}-sub-${Date.now()}`,
        title,
        completed: false,
        taskId,
      };
      setTasks(prev => prev.map(t => {
        if (t.id === taskId) {
          const existing = t.subTasks || [];
          return { ...t, subTasks: [...existing, newSubTask], updatedAt: new Date() };
        }
        return t;
      }));
      return newSubTask;
    } catch (err) {
      setError('Failed to add sub-task');
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const toggleSubTask = async (taskId: string, subTaskId: string): Promise<void> => {
    setIsLoading(true);
    setError(null);
    try {
      setTasks(prev => prev.map(t => {
        if (t.id === taskId) {
          const updatedSubs = (t.subTasks || []).map(st => st.id === subTaskId ? { ...st, completed: !st.completed } : st);
          return { ...t, subTasks: updatedSubs, updatedAt: new Date() };
        }
        return t;
      }));
    } catch (err) {
      setError('Failed to update sub-task');
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const deleteSubTask = async (taskId: string, subTaskId: string): Promise<void> => {
    setIsLoading(true);
    setError(null);
    try {
      setTasks(prev => prev.map(t => {
        if (t.id === taskId) {
          const updatedSubs = (t.subTasks || []).filter(st => st.id !== subTaskId);
          return { ...t, subTasks: updatedSubs, updatedAt: new Date() };
        }
        return t;
      }));
    } catch (err) {
      setError('Failed to delete sub-task');
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const getTaskById = (id: string): Task | undefined => {
    return tasks.find(task => task.id === id);
  };

  const getTasksByStatus = (status: Task['status']): Task[] => {
    return tasks.filter(task => task.status === status);
  };

  const getTasksByPriority = (priority: Task['priority']): Task[] => {
    return tasks.filter(task => task.priority === priority);
  };

  const getOverdueTasks = (): Task[] => {
    const now = new Date();
    return tasks.filter(task => 
      task.dueDate && 
      task.dueDate < now && 
      task.status !== 'completed'
    );
  };

  const getTasksDueToday = (): Task[] => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    
    return tasks.filter(task => 
      task.dueDate && 
      task.dueDate >= today && 
      task.dueDate < tomorrow &&
      task.status !== 'completed'
    );
  };

  const value: TasksContextType = {
    tasks,
    isLoading,
    error,
    createTask,
    updateTask,
    deleteTask,
    completeTask,
    addSubTask,
    toggleSubTask,
    deleteSubTask,
    getTaskById,
    getTasksByStatus,
    getTasksByPriority,
    getOverdueTasks,
    getTasksDueToday,
  };

  return (
    <TasksContext.Provider value={value}>
      {children}
    </TasksContext.Provider>
  );
}; 