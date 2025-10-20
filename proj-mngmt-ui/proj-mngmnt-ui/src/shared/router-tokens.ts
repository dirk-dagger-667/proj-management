export abstract class RoutingConstants {
  public static emptyWildCard: string = '';
  public static universalWildCard: string = '**';
}

export enum ROUTER_TOKENS {
  TASKS_LIST = 'tasks',
  BOARD = 'projects/:id/board',
  TASK_DETAILS = 'tasks/:id',
  DASHBOARD = 'dashboard',
}
