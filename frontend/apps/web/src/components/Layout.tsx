import React from 'react';
import { useAuth } from '../hooks/useAuth';
import { useNavigate, useLocation } from 'react-router-dom';
import { 
  Button, 
  YStack, 
  XStack, 
  Text, 
  Avatar, 
  Separator,
  Sheet,
  SheetTrigger,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from './ui';
import { 
  Home, 
  CheckSquare, 
  Calendar, 
  Users, 
  Settings, 
  LogOut,
  User,
  Bell,
  Plus
} from './icons';

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { user, isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const navigationItems = [
    { icon: Home, label: 'Dashboard', path: '/' },
    { icon: CheckSquare, label: 'Tasks', path: '/tasks' },
    { icon: Calendar, label: 'Calendar', path: '/calendar' },
    { icon: Users, label: 'Groups', path: '/groups' },
    { icon: Settings, label: 'Settings', path: '/settings' },
  ];

  const isActivePath = (path: string) => {
    if (path === '/') {
      return location.pathname === '/';
    }
    return location.pathname.startsWith(path);
  };

  if (!isAuthenticated) {
    return <>{children}</>;
  }

  return (
    <div className="app-container">
      {/* Header */}
      <XStack 
        className="app-header"
        padding="$4" 
        backgroundColor="white" 
        borderBottomWidth={1} 
        borderBottomColor="#e5e7eb"
        justifyContent="space-between"
        alignItems="center"
      >
        <XStack alignItems="center" gap="$3">
          <Text fontSize="$6" fontWeight="bold" color="#111827">
            TodoApp
          </Text>
        </XStack>

        <XStack alignItems="center" gap="$3">
          <Button
            size="small"
            variant="outline"
            icon={Bell}
            circular
          >
            {null}
          </Button>
          
                      <Sheet>
              <SheetTrigger asChild>
                <Button
                  size="small"
                  variant="outline"
                  icon={User}
                  circular
                >
                  {null}
                </Button>
              </SheetTrigger>
              <SheetContent>
                <SheetHeader>
                  <SheetTitle>Profile</SheetTitle>
                </SheetHeader>
                <YStack padding="1rem" gap="0.75rem">
                  <XStack alignItems="center" gap="0.75rem">
                    <Avatar circular size="medium" src={user?.avatar}>
                      {user?.name?.charAt(0).toUpperCase()}
                    </Avatar>
                    <YStack>
                      <Text fontWeight="bold">{user?.name}</Text>
                      <Text color="#6b7280" fontSize="0.875rem">{user?.email}</Text>
                    </YStack>
                  </XStack>
                  <Separator />
                  <Button
                    variant="outline"
                    icon={LogOut}
                    onPress={handleLogout}
                  >
                    Logout
                  </Button>
                </YStack>
              </SheetContent>
            </Sheet>
        </XStack>
      </XStack>

      {/* Main Content */}
      <XStack flex={1} height="calc(100vh - 64px)">
        {/* Sidebar */}
        <div className="sidebar">
          <div className="nav-section">
            <div className="nav-section-title">Main</div>
            <YStack gap="0.25rem">
              {navigationItems.slice(0, 2).map((item) => (
                <button
                  key={item.path}
                  className={`nav-item ${isActivePath(item.path) ? 'active' : ''}`}
                  onClick={() => navigate(item.path)}
                >
                  <item.icon className="nav-icon" />
                  {item.label}
                </button>
              ))}
            </YStack>
          </div>
          
          <div className="nav-section">
            <div className="nav-section-title">Other</div>
            <YStack gap="0.25rem">
              {navigationItems.slice(2).map((item) => (
                <button
                  key={item.path}
                  className={`nav-item ${isActivePath(item.path) ? 'active' : ''}`}
                  onClick={() => navigate(item.path)}
                >
                  <item.icon className="nav-icon" />
                  {item.label}
                </button>
              ))}
            </YStack>
          </div>

          <div className="nav-section">
            <div className="nav-section-title">Projects</div>
            <YStack gap="0.25rem">
              <button className="nav-item">
                <div className="project-color" style={{ backgroundColor: '#ef4444' }}></div>
                Fitness
              </button>
              <button className="nav-item">
                <div className="project-color" style={{ backgroundColor: '#f59e0b' }}></div>
                Groceries
              </button>
              <button className="nav-item">
                <div className="project-color" style={{ backgroundColor: '#3b82f6' }}></div>
                Appointments
              </button>
            </YStack>
            <button className="add-button">
              <Plus size={16} />
              Add project
            </button>
          </div>
          
          <div className="nav-section">
            <div className="nav-section-title">Team</div>
            <YStack gap="0.25rem">
              <button className="nav-item">
                <div className="project-color" style={{ backgroundColor: '#f59e0b' }}></div>
                New Brand
              </button>
              <button className="nav-item">
                <div className="project-color" style={{ backgroundColor: '#3b82f6' }}></div>
                Website Update
              </button>
              <button className="nav-item">
                <div className="project-color" style={{ backgroundColor: '#10b981' }}></div>
                Product Roadmap
              </button>
              <button className="nav-item">
                <div className="project-color" style={{ backgroundColor: '#8b5cf6' }}></div>
                Meeting Agenda
              </button>
            </YStack>
            <button className="add-button">
              <Plus size={16} />
              Add project
            </button>
          </div>
        </div>

        {/* Content Area */}
        <div className="content-wrapper">
          {children}
        </div>
      </XStack>
    </div>
  );
};

export default Layout;