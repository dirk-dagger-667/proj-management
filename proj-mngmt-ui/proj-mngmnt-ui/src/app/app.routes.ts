import { Routes } from '@angular/router';
import { ProjectsDashboardComponent } from '../project-management/projects-dashboard/projects-dashboard';
import { ROUTER_TOKENS, RoutingConstants } from '../shared/router-tokens';
import { TaskDetailsComponent } from '../project-management/tasks-detail/tasks-detail';

export const routes: Routes = [
  { path: ROUTER_TOKENS.DASHBOARD, component: ProjectsDashboardComponent },
  { path: ROUTER_TOKENS.TASK_DETAILS, component: TaskDetailsComponent },
  { path: RoutingConstants.emptyWildCard, component: ProjectsDashboardComponent },
];
