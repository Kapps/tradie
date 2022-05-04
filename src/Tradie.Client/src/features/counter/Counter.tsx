import { ChangeEvent, useState } from 'react';

import { useAppSelector, useAppDispatch } from '../../app/hooks';
import { decrement, increment, incrementByAmount, incrementAsync, incrementIfOdd, selectCount } from './counterSlice';
import styles from './Counter.module.css';
import { Button, Group, Input } from '@mantine/core';

export function Counter() {
  const count = useAppSelector(selectCount);
  const dispatch = useAppDispatch();
  const [incrementAmount, setIncrementAmount] = useState('2');

  const incrementValue = Number(incrementAmount) || 0;

  return (
    <div>
      <div className={styles.row}>
        <Button size="xs" className={styles.button} aria-label="Decrement value" onClick={() => dispatch(decrement())}>
          -
        </Button>
        <span className={styles.value}>{count}</span>
        <Button size="xs" className={styles.button} aria-label="Increment value" onClick={() => dispatch(increment())}>
          +
        </Button>
      </div>
      <div className={styles.row}>
        <Input
          color="secondary"
          required
          className={styles.textbox}
          aria-label="Set increment amount"
          value={incrementAmount}
          onChange={(e: ChangeEvent<HTMLInputElement>) => setIncrementAmount(e.target.value)}
        />
        <Group>
          <Button className={styles.button} onClick={() => dispatch(incrementByAmount(incrementValue))}>
            Add Amount
          </Button>
          <Button className={styles.asyncButton} onClick={() => dispatch(incrementAsync(incrementValue))}>
            Add Async
          </Button>
          <Button className={styles.button} onClick={() => dispatch(incrementIfOdd(incrementValue))}>
            Add If Odd
          </Button>
        </Group>
      </div>
    </div>
  );
}
