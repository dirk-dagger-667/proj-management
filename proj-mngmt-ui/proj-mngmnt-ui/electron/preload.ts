import { contextBridge, ipcRenderer } from 'electron';

contextBridge.exposeInMainWorld('api', {
  ping: () => 'pong',
  syncTask: (task: any) => ipcRenderer.invoke('sync-task', task),
});
