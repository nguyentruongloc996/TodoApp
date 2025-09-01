import React from 'react';
import { Task } from '../hooks/useTasks';
import { 
  YStack, 
  XStack, 
  Text, 
  Card, 
  CardContent,
  Badge,
  Button,
} from './ui';
import { 
  CheckCircle, 
  Circle, 
  Edit, 
  Trash2, 
  Calendar,
  AlertTriangle,
} from './icons';
import { format } from 'date-fns';

interface TaskItemProps {
  task: Task;
  onComplete: (taskId: string) => void;
  onDelete: (taskId: string) => void;
  onEdit: (taskId: string) => void;
}

const TaskItem: React.FC<TaskItemProps> = ({ task, onComplete, onDelete, onEdit }) => {
  const isCompleted = task.status === 'completed';
  const isOverdue = task.dueDate && new Date(task.dueDate) < new Date() && !isCompleted;

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

  return (
    <Card>
      <CardContent padding="$4">
        <XStack justifyContent="space-between" alignItems="flex-start" gap="$3">
          <XStack flex={1} gap="$3" alignItems="flex-start">
            <Button
              variant="ghost"
              size="small"
              icon={isCompleted ? CheckCircle : Circle}
              onPress={() => onComplete(task.id)}
              circular
            >
              {null}
            </Button>
            
            <YStack flex={1} gap="$2">
              <XStack alignItems="center" gap="$2" flexWrap="wrap">
                <Text 
                  fontSize="$4" 
                  fontWeight="bold"
                  textDecorationLine={isCompleted ? 'line-through' : 'none'}
                  color={isCompleted ? '$gray11' : '$color'}
                >
                  {task.title}
                </Text>
                
                <Badge
                  backgroundColor={getPriorityColor(task.priority) + '2'}
                  color={getPriorityColor(task.priority)}
                  size="small"
                >
                  {task.priority}
                </Badge>
                
                <Badge
                  backgroundColor={getStatusColor(task.status) + '2'}
                  color={getStatusColor(task.status)}
                  size="small"
                >
                  {task.status}
                </Badge>
              </XStack>
              
              {task.description && (
                <Text 
                  fontSize="$3" 
                  color="$gray11"
                  textDecorationLine={isCompleted ? 'line-through' : 'none'}
                >
                  {task.description}
                </Text>
              )}
              
              {task.dueDate && (
                <XStack alignItems="center" gap="$2">
                  <Calendar size={14} color="$gray10" />
                  <Text fontSize="$2" color="$gray11">
                    Due: {format(new Date(task.dueDate), 'MMM dd, yyyy')}
                  </Text>
                  {isOverdue && (
                    <AlertTriangle size={14} color="$red10" />
                  )}
                </XStack>
              )}
            </YStack>
          </XStack>
          
          <XStack gap="$1">
            <Button
              variant="ghost"
              size="small"
              icon={Edit}
              onPress={() => onEdit(task.id)}
              circular
            >
              {null}
            </Button>
            <Button
              variant="ghost"
              size="small"
              icon={Trash2}
              onPress={() => onDelete(task.id)}
              circular
            >
              {null}
            </Button>
          </XStack>
        </XStack>
      </CardContent>
    </Card>
  );
};

export default TaskItem; 