import { showNotification } from '@mantine/notifications';
import { createAsyncThunk, createSlice, SerializedError } from '@reduxjs/toolkit';
import { ImBlocked, ImXing } from 'react-icons/im';
import { RootState } from '../../app/store';

export const notifyError = (title: string, error?: SerializedError) => {
  console.error(error);
  showNotification({
    title,
    message: 'Please refresh the page and try again',
    autoClose: false,
    color: 'red',
    icon: ImBlocked({}),
  });
};
