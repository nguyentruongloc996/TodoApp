import React, { useState } from 'react';
import { SubTask } from '../hooks/useTasks';
import { 
  Button, 
  Input, 
  YStack, 
  XStack, 
  Text, 
  Card,
  Checkbox,
  Alert,
  AlertText,
} from './ui';
import { Plus, Trash2, Edit2, Check, X } from './icons';

interface SubTaskItemProps {
  subTask: SubTask;
  onToggle: () => void;
  onDelete: () => void;
  onEdit: (title: string) => void;
  disabled?: boolean;
}

const SubTaskItem: React.FC<SubTaskItemProps> = ({
  subTask,
  onToggle,
  onDelete,
  onEdit,
  disabled = false,
}) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState(subTask.title);

  const handleSaveEdit = () => {
    if (editTitle.trim()) {
      onEdit(editTitle.trim());
      setIsEditing(false);
    }
  };

  const handleCancelEdit = () => {
    setEditTitle(subTask.title);
    setIsEditing(false);
  };

  return (
    <Card padding="$3" backgroundColor={subTask.completed ? "$gray2" : "$background"}>
      <XStack gap="$3" alignItems="center">
        <Checkbox
          checked={subTask.completed}
          onCheckedChange={onToggle}
          disabled={disabled}
        />

        {isEditing ? (
          <XStack flex={1} gap="$2" alignItems="center">
            <Input
              flex={1}
              value={editTitle}
              onChangeText={setEditTitle}
              placeholder="Sub-task title"
              autoFocus
              onSubmitEditing={handleSaveEdit}
            />
            <Button
              size="small"
              variant="ghost"
              icon={Check}
              onPress={handleSaveEdit}
              disabled={!editTitle.trim()}
            />
            <Button
              size="small"
              variant="ghost"
              icon={X}
              onPress={handleCancelEdit}
            />
          </XStack>
        ) : (
          <XStack flex={1} justifyContent="space-between" alignItems="center">
            <Text 
              flex={1}
              textDecorationLine={subTask.completed ? 'line-through' : 'none'}
              color={subTask.completed ? '$gray11' : '$color'}
            >
              {subTask.title}
            </Text>

            <XStack gap="$1">
              <Button
                size="small"
                variant="ghost"
                icon={Edit2}
                onPress={() => setIsEditing(true)}
                disabled={disabled}
              />
              <Button
                size="small"
                variant="ghost"
                icon={Trash2}
                onPress={onDelete}
                disabled={disabled}
              />
            </XStack>
          </XStack>
        )}
      </XStack>
    </Card>
  );
};

interface SubTaskManagerProps {
  subTasks: SubTask[];
  onAdd: (title: string) => Promise<void>;
  onToggle: (subTaskId: string) => Promise<void>;
  onDelete: (subTaskId: string) => Promise<void>;
  onEdit: (subTaskId: string, title: string) => Promise<void>;
  isLoading?: boolean;
  error?: string;
  disabled?: boolean;
}

export const SubTaskManager: React.FC<SubTaskManagerProps> = ({
  subTasks,
  onAdd,
  onToggle,
  onDelete,
  onEdit,
  isLoading = false,
  error,
  disabled = false,
}) => {
  const [newSubTaskTitle, setNewSubTaskTitle] = useState('');
  const [isAdding, setIsAdding] = useState(false);

  const handleAddSubTask = async () => {
    if (!newSubTaskTitle.trim()) return;

    setIsAdding(true);
    try {
      await onAdd(newSubTaskTitle.trim());
      setNewSubTaskTitle('');
    } catch (err) {
      console.error('Failed to add sub-task:', err);
    } finally {
      setIsAdding(false);
    }
  };

  const handleToggleSubTask = async (subTaskId: string) => {
    try {
      await onToggle(subTaskId);
    } catch (err) {
      console.error('Failed to toggle sub-task:', err);
    }
  };

  const handleDeleteSubTask = async (subTaskId: string) => {
    try {
      await onDelete(subTaskId);
    } catch (err) {
      console.error('Failed to delete sub-task:', err);
    }
  };

  const handleEditSubTask = async (subTaskId: string, title: string) => {
    try {
      await onEdit(subTaskId, title);
    } catch (err) {
      console.error('Failed to edit sub-task:', err);
    }
  };

  const completedCount = subTasks.filter(st => st.completed).length;
  const totalCount = subTasks.length;
  const completionPercentage = totalCount > 0 ? Math.round((completedCount / totalCount) * 100) : 0;

  return (
    <YStack gap="$4">
      <YStack gap="$2">
        <XStack justifyContent="space-between" alignItems="center">
          <Text fontSize="$4" fontWeight="600">
            Sub-tasks
          </Text>
          {totalCount > 0 && (
            <Text fontSize="$3" color="$gray11">
              {completedCount}/{totalCount} ({completionPercentage}%)
            </Text>
          )}
        </XStack>

        {totalCount > 0 && (
          <div
            style={{
              width: '100%',
              height: '4px',
              backgroundColor: 'var(--gray4)',
              borderRadius: '2px',
              overflow: 'hidden',
            }}
          >
            <div
              style={{
                width: `${completionPercentage}%`,
                height: '100%',
                backgroundColor: 'var(--green9)',
                transition: 'width 0.3s ease',
              }}
            />
          </div>
        )}
      </YStack>

      {error && (
        <Alert backgroundColor="$red2" borderColor="$red6">
          <AlertText color="$red11">{error}</AlertText>
        </Alert>
      )}

      <YStack gap="$3">
        {subTasks.map((subTask) => (
          <SubTaskItem
            key={subTask.id}
            subTask={subTask}
            onToggle={() => handleToggleSubTask(subTask.id)}
            onDelete={() => handleDeleteSubTask(subTask.id)}
            onEdit={(title) => handleEditSubTask(subTask.id, title)}
            disabled={disabled || isLoading}
          />
        ))}

        {subTasks.length === 0 && !isLoading && (
          <Text color="$gray11" textAlign="center" padding="$4">
            No sub-tasks yet. Add one below!
          </Text>
        )}
      </YStack>

      <XStack gap="$2">
        <Input
          flex={1}
          value={newSubTaskTitle}
          onChangeText={setNewSubTaskTitle}
          placeholder="Add a sub-task..."
          disabled={disabled || isLoading || isAdding}
          onSubmitEditing={handleAddSubTask}
        />
        <Button
          icon={Plus}
          onPress={handleAddSubTask}
          disabled={disabled || isLoading || isAdding || !newSubTaskTitle.trim()}
          loading={isAdding}
        >
          Add
        </Button>
      </XStack>
    </YStack>
  );
};
