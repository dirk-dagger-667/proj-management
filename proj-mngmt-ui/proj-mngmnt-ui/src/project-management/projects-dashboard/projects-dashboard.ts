import { Component, signal } from '@angular/core';
import { ProjectDto, TaskListItem } from '../../client/models';
import { ProjectsService } from '../../client';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TasksList } from '../tasks-list/tasks-list';
import { ProjectsBoardComponent } from '../projects-board/projects-board';
import { Router } from '@angular/router';
import { toObservable } from '@angular/core/rxjs-interop';
import { catchError, debounce, debounceTime, switchMap, tap } from 'rxjs/operators';
import { combineLatest, of } from 'rxjs';

@Component({
  selector: 'app-projects-dashboard',
  imports: [
    MatSidenavModule,
    MatListModule,
    MatTabsModule,
    MatIconModule,
    MatProgressSpinnerModule,
    TasksList,
    ProjectsBoardComponent,
  ],
  templateUrl: './projects-dashboard.html',
  styleUrl: './projects-dashboard.scss',
})
export class ProjectsDashboardComponent {
  // Keeping State
  public projects = signal<ProjectDto[]>([]);
  public selectedProject = signal<ProjectDto | null>(null);
  public tasks = signal<TaskListItem[]>([]);
  public loading = signal<boolean>(false);
  public searchTerm = signal<string>('');
  public page = signal<number>(1);
  public pageSize = signal<number>(40);

  selectProject(project: ProjectDto | null): void {
    this.selectedProject.set(project);
  }

  constructor(private projectService: ProjectsService, private router: Router) {
    this.projectService.apiProjectsGet().subscribe((projects) => {
      this.projects.set(projects);

      if (projects.length > 0) this.selectProject(projects.length > 0 ? projects[0] : null);
    });

    const $project = toObservable(this.selectedProject);
    const $term = toObservable(this.searchTerm);
    const $page = toObservable(this.page);
    const $pageSize = toObservable(this.pageSize);

    // Searching/Filtering on changes to term, page, pageSize, project

    $term
      .pipe(
        debounceTime(400),
        tap(() => this.loading.set(true)),
        switchMap((term) => {
          return this.projectService
            .apiProjectsProjectIdTasksGet(
              this.selectedProject()?.id || '',
              term,
              this.page(),
              this.pageSize()
            )
            .pipe(
              catchError((error) => {
                this.tasks.set([]);
                return of({ tasks: [] });
              })
            );
        }),
        tap((response) => this.tasks.set(response.tasks || [])),
        tap(() => this.loading.set(false))
      )
      .subscribe();

    combineLatest([$project, $page, $pageSize])
      .pipe(
        tap(() => this.loading.set(true)),
        switchMap(([project, page, pageSize]) => {
          if (!project) return [];
          return this.projectService
            .apiProjectsProjectIdTasksGet(project?.id || '', '', page, pageSize)
            .pipe(
              catchError((error) => {
                this.tasks.set([]);
                return of({ tasks: [] });
              })
            );
        }),
        tap((response) => this.tasks.set(response.tasks || [])),
        tap(() => this.loading.set(false))
      )
      .subscribe();
  }

  onTaskSelected(taskId: string) {
    this.router.navigate(['/tasks', taskId]);
  }

  onSearch(searchTerm: string) {
    this.page.set(1);
    this.searchTerm.set(searchTerm);
  }
}
