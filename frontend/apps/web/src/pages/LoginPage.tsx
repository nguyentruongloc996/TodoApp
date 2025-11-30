import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import {
  Box,
  Button,
  TextField,
  Typography,
  Card,
  CardContent,
  Divider,
  Alert,
  InputAdornment,
  IconButton,
  Stack,
  Container,
} from '@mui/material';
import {
  Email as MailIcon,
  Lock as LockIcon,
  Visibility as EyeIcon,
  VisibilityOff as EyeOffIcon,
} from '@mui/icons-material';
import { Github } from '../components/icons';

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
    <Container maxWidth="sm">
      <Box
        sx={{
          minHeight: '100vh',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          py: 4,
        }}
      >
        <Card sx={{ width: '100%', boxShadow: 3 }}>
          <CardContent sx={{ p: 4 }}>
            <Stack spacing={3}>
              {/* Header */}
              <Box textAlign="center">
                <Typography variant="h4" fontWeight="bold" gutterBottom>
                  TodoApp
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {isLogin ? 'Sign in to your account' : 'Create a new account'}
                </Typography>
              </Box>

              {/* Error Alert */}
              {error && (
                <Alert severity="error" onClose={() => setError(null)}>
                  {error}
                </Alert>
              )}

              {/* Form */}
              <form onSubmit={handleSubmit}>
                <Stack spacing={2}>
                  {!isLogin && (
                    <TextField
                      fullWidth
                      label="Name"
                      placeholder="Enter your name"
                      value={formData.name}
                      onChange={(e) => handleInputChange('name', e.target.value)}
                      required
                      variant="outlined"
                    />
                  )}

                  <TextField
                    fullWidth
                    label="Email"
                    type="email"
                    placeholder="Enter your email"
                    value={formData.email}
                    onChange={(e) => handleInputChange('email', e.target.value)}
                    required
                    variant="outlined"
                    InputProps={{
                      startAdornment: (
                        <InputAdornment position="start">
                          <MailIcon color="action" />
                        </InputAdornment>
                      ),
                    }}
                  />

                  <TextField
                    fullWidth
                    label="Password"
                    type={showPassword ? 'text' : 'password'}
                    placeholder="Enter your password"
                    value={formData.password}
                    onChange={(e) => handleInputChange('password', e.target.value)}
                    required
                    variant="outlined"
                    InputProps={{
                      startAdornment: (
                        <InputAdornment position="start">
                          <LockIcon color="action" />
                        </InputAdornment>
                      ),
                      endAdornment: (
                        <InputAdornment position="end">
                          <IconButton
                            onClick={() => setShowPassword(!showPassword)}
                            edge="end"
                            aria-label="toggle password visibility"
                          >
                            {showPassword ? <EyeOffIcon /> : <EyeIcon />}
                          </IconButton>
                        </InputAdornment>
                      ),
                    }}
                  />

                  <Button
                    type="submit"
                    variant="contained"
                    fullWidth
                    size="large"
                    disabled={isLoading}
                    sx={{ mt: 1 }}
                  >
                    {isLogin ? 'Sign In' : 'Sign Up'}
                  </Button>
                </Stack>
              </form>

              {/* Divider */}
              <Divider>OR</Divider>

              {/* Google Login */}
              <Button
                variant="outlined"
                fullWidth
                size="large"
                onClick={handleGoogleLogin}
                disabled={isLoading}
                startIcon={<Github size={20} />}
              >
                Continue with Google
              </Button>

              {/* Toggle Sign In/Up */}
              <Box textAlign="center">
                <Typography variant="body2" component="span" color="text.secondary">
                  {isLogin ? "Don't have an account?" : "Already have an account?"}
                </Typography>
                {' '}
                <Button
                  variant="text"
                  onClick={() => setIsLogin(!isLogin)}
                  disabled={isLoading}
                  sx={{ textTransform: 'none' }}
                >
                  {isLogin ? 'Sign Up' : 'Sign In'}
                </Button>
              </Box>
            </Stack>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
};

export default LoginPage; 