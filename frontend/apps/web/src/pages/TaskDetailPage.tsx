import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTasks } from '../hooks/useTasks';
import { SubTaskManager } from '../components/SubTaskManager';
import { DateTimePicker, TaskStatusSelector, PrioritySelector, ReminderPicker } from '../components/TaskControls';
import { 
  Button, 
  Input, 
  YStack, 
  XStack, 
  Text, 
  Card, 
  CardContent,
  Separator,
  Badge,
  Alert,
  AlertText,
  TextArea,
} from '../components/ui';
import { 
  ArrowLeft, 
  Edit, 
  Save, 
  Trash2, 
  Calendar, 
  Clock, 
  X,
} from '../components/icons';
import { format } from 'date-fns';

const TaskDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { getTaskById, updateTask, deleteTask, addSubTask, toggleSubTask, deleteSubTask, error, isLoading } = useTasks();
  
  const [isEditing, setIsEditing] = useState(false);
  const [task, setTask] = useState(getTaskById(id!));
  const [editedTask, setEditedTask] = useState(task);

  useEffect(() => {
    const currentTask = getTaskById(id!);
    setTask(currentTask);
    setEditedTask(currentTask);
  }, [id, getTaskById]);

  if (!task) {
    return (
      <YStack flex={1} justifyContent="center" alignItems="center">
        <Text>Task not found</Text>
        <Button onPress={() => navigate('/')}>Go Back</Button>
      </YStack>
    );
  }

  const handleSave = async () => {
    try {
      const updatedTask = await updateTask({
        id: task.id,
        title: editedTask?.title,
        description: editedTask?.description,
        status: editedTask?.status,
        priority: editedTask?.priority,
        dueDate: editedTask?.dueDate,
      });
      setTask(updatedTask);
      setEditedTask(updatedTask);
      setIsEditing(false);
    } catch (err) {
      console.error('Failed to update task:', err);
    }
  };

  const handleDelete = async () => {
    if (confirm('Are you sure you want to delete this task?')) {
      try {
        await deleteTask(task.id);
        navigate('/');
      } catch (err) {
        console.error('Failed to delete task:', err);
      }
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'high': return '$red10';
      case 'medium': return '$orange10';
      case 'low': return '$green10';
      default: return '$gray10';
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed': return '$green10';
      case 'in-progress': return '$blue10';
      case 'pending': return '$orange10';
      default: return '$gray10';
    }
  };

  const isOverdue = task.dueDate && new Date(task.dueDate) < new Date() && task.status !== 'completed';

  return (
    <YStack gap="$6">
      {/* Header */}
      <XStack justifyContent="space-between" alignItems="center">
        <XStack alignItems="center" gap="$3">
          <Button
            variant="outline"
            icon={ArrowLeft}
            onPress={() => navigate('/')}
          >
            {null}
          </Button>
          <YStack>
            <Text fontSize="$6" fontWeight="bold">Task Details</Text>
            <Text color="$gray11">View and edit task information</Text>
          </YStack>
        </XStack>
        
        <XStack gap="$2">
          {isEditing ? (
            <>
              <Button
                variant="outline"
                icon={X}
                onPress={() => {
                  setIsEditing(false);
                  setEditedTask(task);
                }}
              >
                Cancel
              </Button>
              <Button
                icon={Save}
                onPress={handleSave}
              >
                Save
              </Button>
            </>
          ) : (
            <>
              <Button
                variant="outline"
                icon={Edit}
                onPress={() => setIsEditing(true)}
              >
                Edit
              </Button>
              <Button
                variant="outline"
                icon={Trash2}
                onPress={handleDelete}
              >
                Delete
              </Button>
            </>
          )}
        </XStack>
      </XStack>

      {error && (
        <Alert backgroundColor="$red2" borderColor="$red6">
          <AlertText color="$red11">{error}</AlertText>
        </Alert>
      )}

      {/* Task Content */}
      <Card>
        <CardContent padding="$6">
          <YStack gap="$6">
            {/* Title */}
            <YStack gap="$2">
              <Text fontSize="$3" color="$gray11" fontWeight="bold">Title</Text>
              {isEditing ? (
                <Input
                  value={editedTask?.title || ''}
                  onChangeText={(value) => setEditedTask(prev => ({ ...prev!, title: value }))}
                  placeholder="Enter task title"
                />
              ) : (
                <Text fontSize="$5" fontWeight="bold">{task.title}</Text>
              )}
            </YStack>

            {/* Status and Priority */}
            <XStack gap="$6" flexWrap="wrap">
              <YStack gap="$3" flex={1}>
                <Text fontSize="$3" color="$gray11" fontWeight="bold">Status</Text>
                {isEditing ? (
                  <TaskStatusSelector
                    value={editedTask?.status || 'pending'}
                    onChange={(status) => setEditedTask(prev => ({ ...prev!, status }))}
                  />
                ) : (
                  <Badge
                    backgroundColor={getStatusColor(task.status) + '2'}
                    color={getStatusColor(task.status)}
                  >
                    {task.status.charAt(0).toUpperCase() + task.status.slice(1).replace('-', ' ')}
                  </Badge>
                )}
              </YStack>

              <YStack gap="$3" flex={1}>
                <Text fontSize="$3" color="$gray11" fontWeight="bold">Priority</Text>
                {isEditing ? (
                  <PrioritySelector
                    value={editedTask?.priority || 'medium'}
                    onChange={(priority) => setEditedTask(prev => ({ ...prev!, priority }))}
                  />
                ) : (
                  <Badge
                    backgroundColor={getPriorityColor(task.priority) + '2'}
                    color={getPriorityColor(task.priority)}
                  >
                    {task.priority.charAt(0).toUpperCase() + task.priority.slice(1)}
                  </Badge>
                )}
              </YStack>
            </XStack>

            {/* Description */}
            <YStack gap="$2">
              <Text fontSize="$3" color="$gray11" fontWeight="bold">Description</Text>
              {isEditing ? (
                <TextArea
                  value={editedTask?.description || ''}
                  onChangeText={(value) => setEditedTask(prev => ({ ...prev!, description: value }))}
                  placeholder="Enter task description"
                  minHeight="100px"
                />
              ) : (
                <Text fontSize="$4" color="$gray11">
                  {task.description || 'No description provided'}
                </Text>
              )}
            </YStack>

            {/* Due Date */}
            <YStack gap="$3">
              <Text fontSize="$3" color="$gray11" fontWeight="bold">Due Date</Text>
              {isEditing ? (
                <DateTimePicker
                  value={editedTask?.dueDate}
                  onChange={(date) => setEditedTask(prev => ({ ...prev!, dueDate: date || undefined }))}
                  placeholder="Select due date"
                  minDate={new Date()}
                />
              ) : (
                <XStack alignItems="center" gap="$2">
                  <Calendar size={16} color="$gray10" />
                  <Text fontSize="$4" color="$gray11">
                    {task.dueDate ? format(new Date(task.dueDate), 'MMM dd, yyyy') : 'No due date'}
                  </Text>
                  {isOverdue && (
                    <Badge backgroundColor="$red2" color="$red11">
                      Overdue
                    </Badge>
                  )}
                </XStack>
              )}
            </YStack>

            {/* Reminder */}
            <YStack gap="$3">
              <Text fontSize="$3" color="$gray11" fontWeight="bold">Reminder</Text>
              {isEditing ? (
                <ReminderPicker
                  dueDate={editedTask?.dueDate}
                  value={editedTask?.reminder}
                  onChange={(reminder) => setEditedTask(prev => ({ ...prev!, reminder: reminder || undefined }))}
                />
              ) : (
                <XStack alignItems="center" gap="$2">
                  <Clock size={16} color="$gray10" />
                  <Text fontSize="$4" color="$gray11">
                    {task.reminder ? format(new Date(task.reminder), 'MMM dd, yyyy HH:mm') : 'No reminder'}
                  </Text>
                </XStack>
              )}
            </YStack>

            {/* Sub-Tasks */}
            <YStack gap="$4">
              <SubTaskManager
                subTasks={task.subTasks || []}
                onAdd={async (title) => {
                  await addSubTask(task.id, title);
                  const updatedTask = getTaskById(task.id);
                  if (updatedTask) {
                    setTask(updatedTask);
                    setEditedTask(updatedTask);
                  }
                }}
                onToggle={async (subTaskId) => {
                  await toggleSubTask(task.id, subTaskId);
                  const updatedTask = getTaskById(task.id);
                  if (updatedTask) {
                    setTask(updatedTask);
                    setEditedTask(updatedTask);
                  }
                }}
                onDelete={async (subTaskId) => {
                  await deleteSubTask(task.id, subTaskId);
                  const updatedTask = getTaskById(task.id);
                  if (updatedTask) {
                    setTask(updatedTask);
                    setEditedTask(updatedTask);
                  }
                }}
                onEdit={async (subTaskId, title) => {
                  // This would need to be implemented in the useTasks hook
                  console.log('Edit sub-task:', subTaskId, title);
                }}
                isLoading={isLoading}
                error={error || undefined}
                disabled={false}
              />
            </YStack>

            {/* Created and Updated */}
            <Separator />
            <XStack gap="$6" flexWrap="wrap">
              <YStack gap="$2">
                <Text fontSize="$3" color="$gray11" fontWeight="bold">Created</Text>
                <XStack alignItems="center" gap="$2">
                  <Clock size={16} color="$gray10" />
                  <Text fontSize="$3" color="$gray11">
                    {format(new Date(task.createdAt), 'MMM dd, yyyy HH:mm')}
                  </Text>
                </XStack>
              </YStack>

              <YStack gap="$2">
                <Text fontSize="$3" color="$gray11" fontWeight="bold">Last Updated</Text>
                <XStack alignItems="center" gap="$2">
                  <Clock size={16} color="$gray10" />
                  <Text fontSize="$3" color="$gray11">
                    {format(new Date(task.updatedAt), 'MMM dd, yyyy HH:mm')}
                  </Text>
                </XStack>
              </YStack>
            </XStack>
          </YStack>
        </CardContent>
      </Card>
    </YStack>
  );
};

export default TaskDetailPage;