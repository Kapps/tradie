export interface AppConfig {
  apiBaseUrl: string;
  staticBaseUrl: string;
}

enum ConfigKey {
  API_URL = 'API_URL',
  STATIC_URL = 'STATIC_URL',
}

const getRequiredWithLocalDefault = (configKey: ConfigKey, localDefault: string) => {
  const override = process.env[`REACT_APP_${configKey}`];
  if (override === undefined) {
    if (process.env.NODE_ENV === 'development') {
      return localDefault;
    }
    throw new Error(`Missing required config key: ${configKey}`);
  }

  return override;
};

export const appConfig: AppConfig = {
  apiBaseUrl: getRequiredWithLocalDefault(ConfigKey.API_URL, 'http://localhost:5181'),
  staticBaseUrl: getRequiredWithLocalDefault(ConfigKey.STATIC_URL, 'http://localhost:3000'),
};