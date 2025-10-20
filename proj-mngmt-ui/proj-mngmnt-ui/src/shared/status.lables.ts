import { Status } from '../client/models';

export const StatusLabels: Record<Status, string> = {
  [Status.ToDo]: 'To Do',
  [Status.InProgress]: 'In Progress',
  [Status.ReadyForTest]: 'Read for Test',
  [Status.Done]: 'Done',
};
