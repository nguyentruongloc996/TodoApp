import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { 
  Button, 
  Input, 
  YStack, 
  XStack, 
  Text, 
  Card, 
  CardHeader, 
  CardContent, 
  Separator,
  Alert,
  AlertText,
} from '../components/ui';
import { Mail, Lock, Eye, EyeOff, Github } from '../components/icons';

const LoginPage: React.FC = () => {
  const navigate = useNavigate();
  const { login, loginWithGoogle, isLoading, register } = useAuth();
  const [isLogin, setIsLogin] = useState(true);
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    name: '',
  });

  const handleInputChange = (field: string, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    setError(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      if (isLogin) {
        await login(formData.email, formData.password);
      } else {
        await register(formData.email, formData.password, formData.name);
      }
      navigate('/');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const handleGoogleLogin = async () => {
    try {
      await loginWithGoogle();
      navigate('/');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Google login failed');
    }
  };

  return (
    <YStack 
      flex={1} 
      justifyContent="center" 
      alignItems="center" 
      padding="$4"
      backgroundColor="$background"
      minHeight="100vh"
    >
      <Card width="100%" maxWidth={400} elevation={4}>
        <CardHeader padding="$6">
          <YStack alignItems="center" gap="$2">
            <Text fontSize="$8" fontWeight="bold" color="$color">
              TodoApp
            </Text>
            <Text fontSize="$4" color="$gray11">
              {isLogin ? 'Sign in to your account' : 'Create a new account'}
            </Text>
          </YStack>
        </CardHeader>

        <CardContent padding="$6">
          <YStack gap="$4">
            {error && (
              <Alert backgroundColor="$red2" borderColor="$red6">
                <AlertText color="$red11">{error}</AlertText>
              </Alert>
            )}

            <form onSubmit={handleSubmit}>
              <YStack gap="$4">
                {!isLogin && (
                  <Input
                    label="Name"
                    placeholder="Enter your name"
                    value={formData.name}
                    onChangeText={(value) => handleInputChange('name', value)}
                    required
                  />
                )}

                <Input
                  label="Email"
                  placeholder="Enter your email"
                  value={formData.email}
                  onChangeText={(value) => handleInputChange('email', value)}
                  icon={Mail}
                  type="email"
                  required
                />

                <Input
                  label="Password"
                  placeholder="Enter your password"
                  value={formData.password}
                  onChangeText={(value) => handleInputChange('password', value)}
                  icon={Lock}
                  type={showPassword ? 'text' : 'password'}
                  endIcon={showPassword ? EyeOff : Eye}
                  onEndIconPress={() => setShowPassword(!showPassword)}
                  required
                />

                <Button
                  type="submit"
                  disabled={isLoading}
                  loading={isLoading}
                >
                  {isLogin ? 'Sign In' : 'Sign Up'}
                </Button>
              </YStack>
            </form>

            <Separator />

            <Button
              variant="outline"
              icon={Github}
              onPress={handleGoogleLogin}
              disabled={isLoading}
            >
              Continue with Google
            </Button>

            <XStack justifyContent="center" gap="$2">
              <Text color="$gray11">
                {isLogin ? "Don't have an account?" : "Already have an account?"}
              </Text>
              <Button
                variant="ghost"
                onPress={() => setIsLogin(!isLogin)}
                disabled={isLoading}
              >
                {isLogin ? 'Sign Up' : 'Sign In'}
              </Button>
            </XStack>
          </YStack>
        </CardContent>
      </Card>
    </YStack>
  );
};

export default LoginPage; 