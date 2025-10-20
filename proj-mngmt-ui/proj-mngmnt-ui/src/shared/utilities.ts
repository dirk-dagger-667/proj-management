import { signal, effect } from '@angular/core';

export function debouncedSignal<T>(source: () => T, delay: number): () => T {
  const debounced = signal<T>(source());
  let timeout: any;

  effect(() => {
    const value = source();
    clearTimeout(timeout);

    timeout = setTimeout(() => {
      debounced.update(() => structuredClone(value));
    }, delay);
  });

  return debounced.asReadonly();
}
