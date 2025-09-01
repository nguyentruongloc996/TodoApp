export type AppConfig = {
  apiBaseUrl: string
}

export const appConfig: AppConfig = {
  apiBaseUrl: (import.meta as any).env?.VITE_API_URL ?? '/api',
}

export const getApiUrl = (path: string): string => {
  const base = appConfig.apiBaseUrl.replace(/\/$/, '')
  const suffix = path.startsWith('/') ? path : `/${path}`
  return `${base}${suffix}`
}

