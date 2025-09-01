import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { YStack, Spinner, Text } from './ui';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <YStack 
        flex={1} 
        justifyContent="center" 
        alignItems="center" 
        height="100vh"
        gap="$4"
      >
        <Spinner size="large" />
        <Text>Loading...</Text>
      </YStack>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute; 