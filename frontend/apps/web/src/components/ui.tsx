import React from 'react';

// Basic Button Component
interface ButtonProps {
  children?: React.ReactNode;
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost';
  size?: 'small' | 'medium' | 'large';
  icon?: React.ComponentType<{ size?: number }>;
  onPress?: () => void;
  disabled?: boolean;
  loading?: boolean;
  type?: 'button' | 'submit' | 'reset';
  circular?: boolean;
  [key: string]: any;
}

export const Button: React.FC<ButtonProps> = ({
  children,
  variant = 'primary',
  size = 'medium',
  icon: Icon,
  onPress,
  disabled = false,
  loading = false,
  type = 'button',
  circular = false,
  ...props
}) => {
  const baseClasses = 'btn';
  const variantClasses = {
    primary: 'btn-primary',
    secondary: 'btn-secondary',
    outline: 'btn-outline',
    ghost: 'btn-ghost',
  };
  const sizeClasses = {
    small: 'btn-sm',
    medium: 'btn-md',
    large: 'btn-lg',
  };

  const className = [
    baseClasses,
    variantClasses[variant],
    sizeClasses[size],
    circular ? 'btn-circular' : '',
    disabled ? 'btn-disabled' : '',
    loading ? 'btn-loading' : '',
  ].filter(Boolean).join(' ');

  return (
    <button
      className={className}
      onClick={onPress}
      disabled={disabled || loading}
      type={type}
      {...props}
    >
      {loading && <span className="btn-spinner" />}
      {Icon && <Icon size={size === 'small' ? 16 : size === 'large' ? 24 : 20} />}
      {children}
    </button>
  );
};

// Basic Input Component
interface InputProps {
  label?: string;
  placeholder?: string;
  value?: string;
  onChangeText?: (value: string) => void;
  type?: string;
  icon?: React.ComponentType<{ size?: number }>;
  endIcon?: React.ComponentType<{ size?: number }>;
  onEndIconPress?: () => void;
  multiline?: boolean;
  required?: boolean;
  width?: string;
  [key: string]: any;
}

export const Input: React.FC<InputProps> = ({
  label,
  placeholder,
  value,
  onChangeText,
  type = 'text',
  icon: Icon,
  endIcon: EndIcon,
  onEndIconPress,
  multiline = false,
  required = false,
  width,
  ...props
}) => {
  const inputClasses = ['input'];
  if (Icon) inputClasses.push('input-with-icon');
  if (EndIcon) inputClasses.push('input-with-end-icon');

  const InputComponent = multiline ? 'textarea' : 'input';

  return (
    <div className="input-container" style={{ width }}>
      {label && (
        <label className="input-label">
          {label}
          {required && <span className="required">*</span>}
        </label>
      )}
      <div className="input-wrapper">
        {Icon && <Icon size={20} />}
        <InputComponent
          className={inputClasses.join(' ')}
          placeholder={placeholder}
          value={value}
          onChange={(e) => onChangeText?.(e.target.value)}
          type={type}
          required={required}
          {...props}
        />
        {EndIcon && (
          <button
            type="button"
            className="input-end-icon"
            onClick={onEndIconPress}
          >
            <EndIcon size={20} />
          </button>
        )}
      </div>
    </div>
  );
};

// Basic Card Component
interface CardProps {
  children: React.ReactNode;
  elevation?: number;
  width?: string | number;
  maxWidth?: string | number;
  minWidth?: string | number;
  height?: string | number;
  maxHeight?: string | number;
  minHeight?: string | number;
  padding?: string;
  margin?: string;
  backgroundColor?: string;
  borderRadius?: string;
  flex?: number;
  [key: string]: any;
}

export const Card: React.FC<CardProps> = ({ 
  children, 
  elevation = 1, 
  width,
  maxWidth,
  minWidth,
  height,
  maxHeight,
  minHeight,
  padding,
  margin,
  backgroundColor,
  borderRadius,
  flex,
  ...otherProps 
}) => {
  // Convert numbers to px strings for CSS properties
  const normalizeSize = (value: string | number | undefined) => {
    if (typeof value === 'number') return `${value}px`;
    return value;
  };

  const style: React.CSSProperties = {
    width: normalizeSize(width),
    maxWidth: normalizeSize(maxWidth),
    minWidth: normalizeSize(minWidth),
    height: normalizeSize(height),
    maxHeight: normalizeSize(maxHeight),
    minHeight: normalizeSize(minHeight),
    padding,
    margin,
    backgroundColor,
    borderRadius,
    flex,
  };

  // Filter out undefined values
  const cleanStyle = Object.fromEntries(
    Object.entries(style).filter(([_, value]) => value !== undefined)
  );

  return (
    <div 
      className={`card card-elevation-${elevation}`} 
      style={cleanStyle}
      {...otherProps}
    >
      {children}
    </div>
  );
};

export const CardHeader: React.FC<{ children: React.ReactNode; padding?: string }> = ({ 
  children, 
  padding = '1rem' 
}) => {
  return (
    <div className="card-header" style={{ padding }}>
      {children}
    </div>
  );
};

export const CardContent: React.FC<{ children: React.ReactNode; padding?: string }> = ({ 
  children, 
  padding = '1rem' 
}) => {
  return (
    <div className="card-content" style={{ padding }}>
      {children}
    </div>
  );
};

// Basic Stack Components
interface StackProps {
  children: React.ReactNode;
  gap?: string;
  flex?: number;
  alignItems?: React.CSSProperties['alignItems'];
  justifyContent?: React.CSSProperties['justifyContent'];
  flexWrap?: React.CSSProperties['flexWrap'];
  padding?: string;
  backgroundColor?: string;
  borderBottomWidth?: number;
  borderBottomColor?: string;
  borderRightWidth?: number;
  borderRightColor?: string;
  width?: string | number;
  height?: string | number;
  overflow?: string;
  minWidth?: string | number;
  minHeight?: string | number;
  maxWidth?: string | number;
  maxHeight?: string | number;
  [key: string]: any;
}

export const YStack: React.FC<StackProps> = ({ 
  children, 
  gap = '0.5rem',
  flex,
  alignItems,
  justifyContent,
  flexWrap,
  padding,
  backgroundColor,
  borderBottomWidth,
  borderBottomColor,
  borderRightWidth,
  borderRightColor,
  width,
  height,
  overflow,
  minWidth,
  minHeight,
  maxWidth,
  maxHeight,
  ...otherProps 
}) => {
  // Convert numbers to px strings for CSS properties
  const normalizeSize = (value: string | number | undefined) => {
    if (typeof value === 'number') return `${value}px`;
    return value;
  };

  const style: React.CSSProperties = {
    gap,
    flex,
    alignItems,
    justifyContent,
    flexWrap,
    padding,
    backgroundColor,
    borderBottomWidth: borderBottomWidth ? `${borderBottomWidth}px` : undefined,
    borderBottomColor,
    borderRightWidth: borderRightWidth ? `${borderRightWidth}px` : undefined,
    borderRightColor,
    width: normalizeSize(width),
    height: normalizeSize(height),
    overflow,
    minWidth: normalizeSize(minWidth),
    minHeight: normalizeSize(minHeight),
    maxWidth: normalizeSize(maxWidth),
    maxHeight: normalizeSize(maxHeight),
  };

  // Filter out undefined values
  const cleanStyle = Object.fromEntries(
    Object.entries(style).filter(([_, value]) => value !== undefined)
  );

  return (
    <div 
      className="stack stack-y" 
      style={cleanStyle}
      {...otherProps}
    >
      {children}
    </div>
  );
};

export const XStack: React.FC<StackProps> = ({ 
  children, 
  gap = '0.5rem',
  flex,
  alignItems,
  justifyContent,
  flexWrap,
  padding,
  backgroundColor,
  borderBottomWidth,
  borderBottomColor,
  borderRightWidth,
  borderRightColor,
  width,
  height,
  overflow,
  minWidth,
  minHeight,
  maxWidth,
  maxHeight,
  ...otherProps 
}) => {
  // Convert numbers to px strings for CSS properties
  const normalizeSize = (value: string | number | undefined) => {
    if (typeof value === 'number') return `${value}px`;
    return value;
  };

  const style: React.CSSProperties = {
    gap,
    flex,
    alignItems,
    justifyContent,
    flexWrap,
    padding,
    backgroundColor,
    borderBottomWidth: borderBottomWidth ? `${borderBottomWidth}px` : undefined,
    borderBottomColor,
    borderRightWidth: borderRightWidth ? `${borderRightWidth}px` : undefined,
    borderRightColor,
    width: normalizeSize(width),
    height: normalizeSize(height),
    overflow,
    minWidth: normalizeSize(minWidth),
    minHeight: normalizeSize(minHeight),
    maxWidth: normalizeSize(maxWidth),
    maxHeight: normalizeSize(maxHeight),
  };

  // Filter out undefined values
  const cleanStyle = Object.fromEntries(
    Object.entries(style).filter(([_, value]) => value !== undefined)
  );

  return (
    <div 
      className="stack stack-x" 
      style={cleanStyle}
      {...otherProps}
    >
      {children}
    </div>
  );
};

// Basic Text Component
interface TextProps {
  children: React.ReactNode;
  fontSize?: string;
  fontWeight?: string;
  color?: string;
  textDecorationLine?: string;
  [key: string]: any;
}

export const Text: React.FC<TextProps> = ({ 
  children, 
  fontSize = '1rem',
  fontWeight = 'normal',
  color = 'inherit',
  textDecorationLine,
  ...props 
}) => {
  return (
    <span 
      style={{ 
        fontSize, 
        fontWeight, 
        color, 
        textDecorationLine,
        ...props 
      }}
    >
      {children}
    </span>
  );
};

// Basic Badge Component
interface BadgeProps {
  children: React.ReactNode;
  variant?: 'filled' | 'outlined';
  backgroundColor?: string;
  color?: string;
  size?: string;
  onPress?: () => void;
  [key: string]: any;
}

export const Badge: React.FC<BadgeProps> = ({ 
  children, 
  variant = 'filled',
  backgroundColor,
  color,
  size = 'medium',
  onPress,
  ...props 
}) => {
  const className = [
    'badge',
    `badge-${variant}`,
    `badge-${size}`,
  ].join(' ');

  const Component = onPress ? 'button' : 'span';

  return (
    <Component
      className={className}
      style={{ backgroundColor, color }}
      onClick={onPress}
      {...props}
    >
      {children}
    </Component>
  );
};

// Basic Avatar Component
interface AvatarProps {
  children?: React.ReactNode;
  circular?: boolean;
  size?: string;
  src?: string;
  [key: string]: any;
}

export const Avatar: React.FC<AvatarProps> = ({ 
  children, 
  circular = true,
  size = 'medium',
  src,
  ...props 
}) => {
  const className = [
    'avatar',
    circular ? 'avatar-circular' : '',
    `avatar-${size}`,
  ].join(' ');

  return (
    <div className={className} {...props}>
      {src && <img src={src} alt="Avatar" className="avatar-image" />}
      {children && <div className="avatar-fallback">{children}</div>}
    </div>
  );
};

// Basic Separator Component
export const Separator: React.FC = () => {
  return <hr className="separator" />;
};

// Basic Spinner Component
interface SpinnerProps {
  size?: 'small' | 'medium' | 'large';
}

export const Spinner: React.FC<SpinnerProps> = ({ size = 'medium' }) => {
  return <div className={`spinner spinner-${size}`} />;
};

// Basic Alert Components
interface AlertProps {
  children: React.ReactNode;
  backgroundColor?: string;
  borderColor?: string;
  [key: string]: any;
}

export const Alert: React.FC<AlertProps> = ({ 
  children, 
  backgroundColor,
  borderColor,
  ...props 
}) => {
  return (
    <div 
      className="alert" 
      style={{ backgroundColor, borderColor }}
      {...props}
    >
      {children}
    </div>
  );
};

export const AlertText: React.FC<{ children: React.ReactNode; color?: string }> = ({ 
  children, 
  color 
}) => {
  return (
    <span className="alert-text" style={{ color }}>
      {children}
    </span>
  );
};

// Basic Sheet Components
interface SheetProps {
  children: React.ReactNode;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
  [key: string]: any;
}

export const Sheet: React.FC<SheetProps> = ({ children, ...props }) => {
  return <div className="sheet" {...props}>{children}</div>;
};

export const SheetTrigger: React.FC<{ children: React.ReactNode; asChild?: boolean }> = ({ 
  children 
}) => {
  return <div className="sheet-trigger">{children}</div>;
};

export const SheetContent: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  return <div className="sheet-content">{children}</div>;
};

export const SheetHeader: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  return <div className="sheet-header">{children}</div>;
};

export const SheetTitle: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  return <h2 className="sheet-title">{children}</h2>;
};

// Basic Switch Component
interface SwitchProps {
  checked?: boolean;
  onCheckedChange?: (checked: boolean) => void;
}

export const Switch: React.FC<SwitchProps> = ({ checked, onCheckedChange }) => {
  return (
    <label className="switch">
      <input
        type="checkbox"
        checked={checked}
        onChange={(e) => onCheckedChange?.(e.target.checked)}
      />
      <span className="switch-slider" />
    </label>
  );
};

// Basic TextArea Component
export const TextArea: React.FC<InputProps & { minHeight?: string }> = ({ 
  minHeight,
  ...props 
}) => {
  return <Input {...props} multiline minHeight={minHeight} />;
};

// Checkbox Component
interface CheckboxProps {
  checked?: boolean;
  onCheckedChange?: (checked: boolean) => void;
  disabled?: boolean;
  label?: string;
  [key: string]: any;
}

export const Checkbox: React.FC<CheckboxProps> = ({
  checked = false,
  onCheckedChange,
  disabled = false,
  label,
  ...props
}) => {
  const handleChange = () => {
    if (!disabled && onCheckedChange) {
      onCheckedChange(!checked);
    }
  };

  return (
    <div className="checkbox-container" style={{ display: 'flex', alignItems: 'center', gap: '8px' }} {...props}>
      <div
        className={`checkbox ${checked ? 'checked' : ''} ${disabled ? 'disabled' : ''}`}
        style={{
          width: '20px',
          height: '20px',
          border: checked ? '2px solid var(--blue9)' : '2px solid var(--gray8)',
          borderRadius: '4px',
          backgroundColor: checked ? 'var(--blue9)' : 'transparent',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          cursor: disabled ? 'not-allowed' : 'pointer',
          transition: 'all 0.2s ease',
          opacity: disabled ? 0.5 : 1,
        }}
        onClick={handleChange}
      >
        {checked && (
          <svg
            width="12"
            height="12"
            viewBox="0 0 24 24"
            fill="none"
            stroke="white"
            strokeWidth="3"
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <polyline points="20,6 9,17 4,12" />
          </svg>
        )}
      </div>
      {label && <span style={{ cursor: disabled ? 'not-allowed' : 'pointer' }} onClick={handleChange}>{label}</span>}
    </div>
  );
};