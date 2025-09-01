import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTasks } from '../hooks/useTasks';
import TaskItem from '../components/TaskItem';
import { 
  Button, 
  Input, 
  YStack, 
  XStack, 
  Text, 
  Card, 
  CardContent,
  Badge,
  Spinner,
  Alert,
  AlertText,
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from '../components/ui';
import { 
  Plus, 
  Calendar, 
  Clock, 
  CheckCircle, 
  AlertTriangle,
  Search,
} from '../components/icons';

const HomePage: React.FC = () => {
  const navigate = useNavigate();
  const { 
    tasks, 
    isLoading, 
    error, 
    createTask, 
    deleteTask, 
    completeTask,
    getTasksByStatus,
    getOverdueTasks,
    getTasksDueToday,
  } = useTasks();

  const [showCreateTask, setShowCreateTask] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<'all' | 'pending' | 'in-progress' | 'completed'>('all');

  const [newTask, setNewTask] = useState({
    title: '',
    description: '',
    priority: 'medium' as const,
    dueDate: '',
    reminder: '',
  });

  const handleCreateTask = async () => {
    try {
      await createTask({
        title: newTask.title,
        description: newTask.description,
        priority: newTask.priority,
        dueDate: newTask.dueDate ? new Date(newTask.dueDate) : undefined,
        reminder: newTask.reminder ? new Date(newTask.reminder) : undefined,
      });
      setNewTask({ title: '', description: '', priority: 'medium', dueDate: '', reminder: '' });
      setShowCreateTask(false);
    } catch (err) {
      console.error('Failed to create task:', err);
    }
  };

  const handleCompleteTask = async (taskId: string) => {
    try {
      await completeTask(taskId);
    } catch (err) {
      console.error('Failed to complete task:', err);
    }
  };

  const handleDeleteTask = async (taskId: string) => {
    try {
      await deleteTask(taskId);
    } catch (err) {
      console.error('Failed to delete task:', err);
    }
  };

  const filteredTasks = tasks.filter(task => {
    const matchesSearch = task.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         task.description?.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || task.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  const pendingTasks = getTasksByStatus('pending');
  // const inProgressTasks = getTasksByStatus('in-progress');
  const completedTasks = getTasksByStatus('completed');
  const overdueTasks = getOverdueTasks();
  const tasksDueToday = getTasksDueToday();

  if (isLoading) {
    return (
      <YStack flex={1} justifyContent="center" alignItems="center">
        <Spinner size="large" />
        <Text>Loading tasks...</Text>
      </YStack>
    );
  }

  return (
    <div>
      {/* Header */}
      <div className="section-header">
        <div>
          <h1 className="page-title">Today</h1>
          <p className="page-subtitle">Manage your tasks and stay organized</p>
        </div>
        <Button
          icon={Plus}
          onPress={() => setShowCreateTask(true)}
        >
          Add task
        </Button>
      </div>

      {error && (
        <Alert backgroundColor="$red2" borderColor="$red6">
          <AlertText color="$red11">{error}</AlertText>
        </Alert>
      )}

      {/* My Projects Section */}
      <div className="nav-section">
        <h2 className="section-title">My Projects</h2>
        <div className="task-list">
          {pendingTasks.slice(0, 3).map((task) => (
            <div key={task.id} className="task-item" onClick={() => navigate(`/tasks/${task.id}`)}>
              <div className="task-header">
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                  <input type="checkbox" onChange={() => handleCompleteTask(task.id)} />
                  <span className="task-title">{task.title}</span>
                  {task.priority && (
                    <span className={`priority-badge priority-${task.priority}`}>
                      {task.priority}
                    </span>
                  )}
                </div>
                <div className="task-meta">
                  {task.dueDate && (
                    <span>
                      <Calendar size={14} />
                      {new Date(task.dueDate).toLocaleDateString()}
                    </span>
                  )}
                  <Clock size={14} />
                  {task.reminder && new Date(task.reminder).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </div>
              </div>
              {task.description && (
                <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>
                  {task.description}
                </p>
              )}
            </div>
          ))}
          
          <button className="add-button" onClick={() => setShowCreateTask(true)}>
            <Plus size={16} />
            Add task
          </button>
        </div>
      </div>

      {/* Stats Cards - Only show if there are stats */}
      {(overdueTasks.length > 0 || tasksDueToday.length > 0 || completedTasks.length > 0) && (
        <div className="stats-grid">
          {overdueTasks.length > 0 && (
            <Card>
              <CardContent padding="$4">
                <XStack alignItems="center" gap="$3">
                  <AlertTriangle color="$orange10" />
                  <YStack>
                    <Text fontSize="$6" fontWeight="bold">{overdueTasks.length}</Text>
                    <Text color="$gray11" fontSize="$3">Overdue</Text>
                  </YStack>
                </XStack>
              </CardContent>
            </Card>
          )}

          {tasksDueToday.length > 0 && (
            <Card>
              <CardContent padding="$4">
                <XStack alignItems="center" gap="$3">
                  <Calendar color="$green10" />
                  <YStack>
                    <Text fontSize="$6" fontWeight="bold">{tasksDueToday.length}</Text>
                    <Text color="$gray11" fontSize="$3">Due Today</Text>
                  </YStack>
                </XStack>
              </CardContent>
            </Card>
          )}

          {completedTasks.length > 0 && (
            <Card>
              <CardContent padding="$4">
                <XStack alignItems="center" gap="$3">
                  <CheckCircle color="$purple10" />
                  <YStack>
                    <Text fontSize="$6" fontWeight="bold">{completedTasks.length}</Text>
                    <Text color="$gray11" fontSize="$3">Completed</Text>
                  </YStack>
                </XStack>
              </CardContent>
            </Card>
          )}
        </div>
      )}

      {/* Search and Filters - Only show if there are many tasks */}
      {tasks.length > 5 && (
        <XStack gap="$4" alignItems="center" flexWrap="wrap" marginBottom="$4">
          <Input
            placeholder="Search tasks..."
            value={searchTerm}
            onChangeText={setSearchTerm}
            icon={Search}
            width="300px"
          />
          
          <XStack gap="$2">
            {(['all', 'pending', 'in-progress', 'completed'] as const).map((status) => (
              <Badge
                key={status}
                variant={statusFilter === status ? 'filled' : 'outlined'}
                onPress={() => setStatusFilter(status)}
                backgroundColor={statusFilter === status ? '$blue5' : 'transparent'}
                color={statusFilter === status ? '$blue11' : '$color'}
              >
                {status.charAt(0).toUpperCase() + status.slice(1)}
              </Badge>
            ))}
          </XStack>
        </XStack>
      )}
              color={statusFilter === status ? '$blue11' : '$color'}
      {/* Remaining Tasks List - if user wants to see all */}
      {(searchTerm || statusFilter !== 'all' || tasks.length > 3) && (
        <YStack gap="$3">
          <Text fontSize="$5" fontWeight="bold">
            All Tasks ({filteredTasks.length})
          </Text>
          
          {filteredTasks.length === 0 ? (
            <Card>
              <CardContent padding="$6">
                <YStack alignItems="center" gap="$3">
                  <Text fontSize="$4" color="$gray11">No tasks found</Text>
                  <Button
                    variant="outline"
                    onPress={() => setShowCreateTask(true)}
                  >
                    Create your first task
                  </Button>
                </YStack>
              </CardContent>
            </Card>
          ) : (
            <YStack gap="$3">
              {filteredTasks.map((task) => (
                <TaskItem
                  key={task.id}
                  task={task}
                  onComplete={() => handleCompleteTask(task.id)}
                  onDelete={() => handleDeleteTask(task.id)}
                  onEdit={() => navigate(`/tasks/${task.id}`)}
                />
              ))}
            </YStack>
          )}
        </YStack>
      )}

      {/* Create Task Sheet */}
      <Sheet open={showCreateTask} onOpenChange={setShowCreateTask}>
        <SheetContent>
          <SheetHeader>
            <SheetTitle>Create New Task</SheetTitle>
          </SheetHeader>
          <YStack padding="$4" gap="$4">
            <Input
              label="Title"
              placeholder="Enter task title"
              value={newTask.title}
              onChangeText={(value) => setNewTask(prev => ({ ...prev, title: value }))}
            />
            
            <Input
              label="Description"
              placeholder="Enter task description"
              value={newTask.description}
              onChangeText={(value) => setNewTask(prev => ({ ...prev, description: value }))}
              multiline
            />
            
            <Input
              label="Due Date"
              type="date"
              value={newTask.dueDate}
              onChangeText={(value) => setNewTask(prev => ({ ...prev, dueDate: value }))}
            />

        <Input
          label="Reminder"
          type="datetime-local"
          value={newTask.reminder}
          onChangeText={(value) => setNewTask(prev => ({ ...prev, reminder: value }))}
        />
            
            <XStack gap="$3">
              <Button
                variant="outline"
                onPress={() => setShowCreateTask(false)}
                flex={1}
              >
                Cancel
              </Button>
              <Button
                onPress={handleCreateTask}
                disabled={!newTask.title.trim()}
                flex={1}
              >
                Create Task
              </Button>
            </XStack>
          </YStack>
        </SheetContent>
      </Sheet>
    </div>
  );
};

export default HomePage; 