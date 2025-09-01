import React, { useState, forwardRef } from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { Calendar, Clock } from './icons';
import { Button, Input, YStack, XStack, Text } from './ui';

interface DateTimePickerProps {
  label?: string;
  value?: Date;
  onChange: (date: Date | null) => void;
  showTimeSelect?: boolean;
  placeholder?: string;
  minDate?: Date;
  maxDate?: Date;
  disabled?: boolean;
  error?: string;
}

// Custom input component for DatePicker
const CustomInput = forwardRef<HTMLInputElement, any>(({ value, onClick, placeholder, error, ...props }, ref) => (
  <Input
    ref={ref}
    value={value}
    onClick={onClick}
    placeholder={placeholder}
    readOnly
    icon={Calendar}
    error={error}
    {...props}
  />
));

CustomInput.displayName = 'CustomInput';

export const DateTimePicker: React.FC<DateTimePickerProps> = ({
  label,
  value,
  onChange,
  showTimeSelect = false,
  placeholder = 'Select date',
  minDate,
  maxDate,
  disabled = false,
  error,
}) => {
  return (
    <YStack gap="$2">
      {label && <Text fontSize="$3" fontWeight="500">{label}</Text>}
      <DatePicker
        selected={value}
        onChange={onChange}
        showTimeSelect={showTimeSelect}
        timeFormat="HH:mm"
        timeIntervals={15}
        dateFormat={showTimeSelect ? "MMM d, yyyy h:mm aa" : "MMM d, yyyy"}
        placeholderText={placeholder}
        minDate={minDate}
        maxDate={maxDate}
        disabled={disabled}
        customInput={<CustomInput error={error} />}
        popperClassName="date-picker-popper"
      />
      {error && <Text color="$red10" fontSize="$2">{error}</Text>}
    </YStack>
  );
};

interface TaskStatusSelectorProps {
  value: 'pending' | 'in-progress' | 'completed';
  onChange: (status: 'pending' | 'in-progress' | 'completed') => void;
  disabled?: boolean;
}

export const TaskStatusSelector: React.FC<TaskStatusSelectorProps> = ({
  value,
  onChange,
  disabled = false,
}) => {
  const statuses = [
    { value: 'pending', label: 'Pending', color: '$gray9' },
    { value: 'in-progress', label: 'In Progress', color: '$blue9' },
    { value: 'completed', label: 'Completed', color: '$green9' },
  ] as const;

  return (
    <XStack gap="$2">
      {statuses.map((status) => (
        <Button
          key={status.value}
          variant={value === status.value ? 'primary' : 'outline'}
          size="small"
          onPress={() => onChange(status.value)}
          disabled={disabled}
        >
          {status.label}
        </Button>
      ))}
    </XStack>
  );
};

interface PrioritySelectorProps {
  value: 'low' | 'medium' | 'high';
  onChange: (priority: 'low' | 'medium' | 'high') => void;
  disabled?: boolean;
}

export const PrioritySelector: React.FC<PrioritySelectorProps> = ({
  value,
  onChange,
  disabled = false,
}) => {
  const priorities = [
    { value: 'low', label: 'Low', color: '$green9' },
    { value: 'medium', label: 'Medium', color: '$yellow9' },
    { value: 'high', label: 'High', color: '$red9' },
  ] as const;

  return (
    <XStack gap="$2">
      {priorities.map((priority) => (
        <Button
          key={priority.value}
          variant={value === priority.value ? 'primary' : 'outline'}
          size="small"
          onPress={() => onChange(priority.value)}
          disabled={disabled}
        >
          {priority.label}
        </Button>
      ))}
    </XStack>
  );
};

interface ReminderPickerProps {
  dueDate?: Date;
  value?: Date;
  onChange: (reminder: Date | null) => void;
  disabled?: boolean;
}

export const ReminderPicker: React.FC<ReminderPickerProps> = ({
  dueDate,
  value,
  onChange,
  disabled = false,
}) => {
  const [showCustom, setShowCustom] = useState(false);

  const quickOptions = [
    { label: '15 minutes before', minutes: 15 },
    { label: '1 hour before', minutes: 60 },
    { label: '1 day before', minutes: 24 * 60 },
    { label: 'Custom', minutes: -1 },
  ];

  const handleQuickOption = (minutes: number) => {
    if (minutes === -1) {
      setShowCustom(true);
      return;
    }

    if (dueDate) {
      const reminder = new Date(dueDate);
      reminder.setMinutes(reminder.getMinutes() - minutes);
      onChange(reminder);
    }
    setShowCustom(false);
  };

  return (
    <YStack gap="$3">
      <YStack gap="$2">
        <Text fontSize="$3" fontWeight="500">Reminder</Text>
        <XStack gap="$2" flexWrap="wrap">
          {quickOptions.map((option) => (
            <Button
              key={option.label}
              variant="outline"
              size="small"
              onPress={() => handleQuickOption(option.minutes)}
              disabled={disabled || (!dueDate && option.minutes !== -1)}
            >
              <Clock size={14} />
              {option.label}
            </Button>
          ))}
        </XStack>
      </YStack>

      {showCustom && (
        <DateTimePicker
          label="Custom Reminder"
          value={value}
          onChange={onChange}
          showTimeSelect
          placeholder="Select reminder date & time"
          maxDate={dueDate}
          disabled={disabled}
        />
      )}

      {value && !showCustom && (
        <YStack gap="$2">
          <Text fontSize="$2" color="$gray11">
            Reminder set for: {value.toLocaleString()}
          </Text>
          <Button
            variant="ghost"
            size="small"
            onPress={() => onChange(null)}
            disabled={disabled}
          >
            Clear reminder
          </Button>
        </YStack>
      )}
    </YStack>
  );
};
