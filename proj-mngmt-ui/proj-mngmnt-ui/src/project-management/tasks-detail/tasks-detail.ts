import {
  Component,
  signal,
  effect,
  inject,
  DestroyRef,
  ViewChild,
  AfterViewInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  Validators,
  FormGroup,
} from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CdkVirtualScrollViewport, ScrollingModule } from '@angular/cdk/scrolling';
import { MatNativeDateModule } from '@angular/material/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import {
  catchError,
  debounceTime,
  filter,
  finalize,
  map,
  startWith,
  switchMap,
  tap,
} from 'rxjs/operators';
import { BehaviorSubject, forkJoin, of } from 'rxjs';
import {
  AuditEntryDto,
  AuditType,
  Priority,
  Status,
  TaskItemDto,
  TaskType,
} from '../../client/models';
import { TasksService } from '../../client/services/tasks.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-task-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatButtonModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    ScrollingModule,
    MatNativeDateModule,
    RouterLink,
  ],
  templateUrl: './tasks-detail.html',
  styleUrls: ['./tasks-detail.scss'],
})
export class TaskDetailsComponent {
  private fb = inject(FormBuilder);
  private taskService = inject(TasksService);
  private route = inject(ActivatedRoute);
  private destroyRef = inject(DestroyRef);
  private _paginator!: MatPaginator;
  private paginatorInitialized = false;

  // State
  task = signal<TaskItemDto | null>(null);
  taskForm: FormGroup;
  audits = signal<AuditEntryDto[]>([]);
  loading = signal<boolean>(false);
  hasMoreAudits = signal<boolean>(true);
  page = signal<number>(1);
  pageSize = signal<number>(10);
  auditsCount = signal<number>(0);

  public taskTypes = Object.values(TaskType).filter((value) => typeof value !== 'number');
  public priorities = Object.values(Priority).filter((value) => typeof value !== 'number');
  public statuses = Object.values(Status).filter((value) => typeof value !== 'number');
  public auditTypes = Object.values(AuditType).filter((value) => typeof value !== 'number');

  displayedColumns = ['auditType', 'metadata', 'createdAt'];

  @ViewChild(MatPaginator) set paginator(p: MatPaginator | undefined) {
    if (!p) return;
    this._paginator = p;
    this.initPaginatorStream();
  }

  constructor() {
    this.taskForm = this.fb.group({
      type: ['', Validators.required],
      title: ['', [Validators.required, Validators.minLength(3), Validators.max(30)]],
      description: ['', Validators.max(500)],
      assignee: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(20)]],
      priority: ['', Validators.required],
      status: ['', Validators.required],
      estimate: [0, [Validators.required, Validators.min(1), Validators.max(21)]],
      createdAt: [null, Validators.required],
    });

    this.route.paramMap
      .pipe(
        takeUntilDestroyed(this.destroyRef), // or inject(DestroyRef)
        tap(() => this.loading.set(true)),
        map((params) => params.get('id')),
        filter((taskId): taskId is string => !!taskId), // narrow to non-null string
        switchMap((taskId) =>
          forkJoin({
            task: this.taskService.apiTasksIdGet(taskId),
            audits: this.taskService.apiTasksTaskIdAuditsGet(
              taskId,
              '',
              this.page(),
              this.pageSize()
            ),
          })
        ),
        tap(({ task, audits }) => {
          this.task.set(task);

          this.taskForm.get('status')?.setValue(Status[task.status!]);
          this.taskForm.get('priority')?.setValue(Priority[task.priority!]);
          this.taskForm.get('type')?.setValue(TaskType[task.type!]);

          this.taskForm.patchValue({
            title: task!.title,
            description: task.description,
            createdAt: task.createdAt,
            estimate: task.estimate,
            assignee: task.assignee,
          });

          this.audits.set(audits.audits || []);
          this.auditsCount.set(audits.count || 0);
        }),
        tap(() => this.loading.set(false))
      )
      .subscribe();
  }

  private initPaginatorStream() {
    if (this.paginatorInitialized) return;
    this.paginatorInitialized = true;

    this._paginator.page
      .pipe(
        startWith({ pageIndex: 0, pageSize: this.pageSize() }),
        tap((page) => {
          this.page.update((p) => page.pageIndex + 1);
        }),
        tap(() => this.loading.set(true)),
        switchMap(() => {
          return this.taskService
            .apiTasksTaskIdAuditsGet(this.task()?.id!, '', this.page(), this.pageSize())
            .pipe(
              catchError((error) => {
                console.log(error);
                return of({ audits: [], count: 0 });
              })
            );
        }),
        map((response) => {
          if (response === null) return [];
          this.auditsCount.set(response?.count || 0);
          return response.audits;
        }),
        tap((audits) => this.audits.set(audits || [])),
        tap(() => this.loading.set(false))
      )
      .subscribe();
  }

  saveTask(): void {
    if (this.taskForm.valid) {
      const updatedTask: TaskItemDto = {
        ...this.task()!,
        ...this.taskForm.getRawValue(),
      };
      this.taskService.apiTasksTaskIdPut(updatedTask.id || '', updatedTask).subscribe({
        next: (task) => {
          this.task.set(task);
          this.taskForm.patchValue(task);
          alert('Task updated successfully');
        },
        error: (err) => {
          if (err.status === 409) {
            alert('Concurrency error: Task was modified by another user. Please reload.');
          } else {
            alert('Failed to update task.');
          }
        },
      });
    }
  }
}
