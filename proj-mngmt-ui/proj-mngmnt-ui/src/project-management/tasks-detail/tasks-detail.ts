import { Component, signal, effect, inject } from '@angular/core';
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
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CdkVirtualScrollViewport, ScrollingModule } from '@angular/cdk/scrolling';
import { MatNativeDateModule } from '@angular/material/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { debounceTime, switchMap } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import {
  AuditEntryDto,
  AuditType,
  Priority,
  Status,
  TaskItemDto,
  TaskType,
} from '../../client/models';
import { TasksService } from '../../client/services/tasks.service';

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

  // State
  task = signal<TaskItemDto | null>(null);
  taskForm: FormGroup;
  audits = signal<AuditEntryDto[]>([]);
  loading = signal<boolean>(false);
  hasMoreAudits = signal<boolean>(true);
  page = signal<number>(1);
  pageSize = signal<number>(50);

  public taskTypes = Object.values(TaskType).filter((value) => typeof value !== 'number');
  public priorities = Object.values(Priority).filter((value) => typeof value !== 'number');
  public statuses = Object.values(Status).filter((value) => typeof value !== 'number');
  public auditTypes = Object.values(AuditType).filter((value) => typeof value !== 'number');

  private scrollTrigger = new BehaviorSubject<void>(undefined);
  displayedColumns = ['auditType', 'metadata', 'createdAt'];

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

    this.route.paramMap.subscribe((params) => {
      const taskId = params.get('id');
      if (taskId) {
        this.taskService.apiTasksIdGet(taskId).subscribe((task) => {
          if (task) {
            this.task.set(task);

            this.taskForm.get('status')?.setValue(Status[task.status!]);
            this.taskForm.get('priority')?.setValue(Priority[task.priority!]);
            this.taskForm.get('type')?.setValue(TaskType[task.type!]);

            this.taskForm.patchValue({
              title: task.title,
              description: task.description,
              createdAt: task.createdAt,
              estimate: task.estimate,
              assignee: task.assignee,
            });
          }
        });
      }
    });

    effect(() => {
      this.scrollTrigger
        .pipe(
          debounceTime(200),
          switchMap(() => {
            this.loading.set(true);
            return this.taskService.apiTasksTaskIdAuditsGet(
              this.task()?.id || '',
              '',
              this.page(),
              this.pageSize()
            );
          })
        )
        .subscribe((audits) => {
          this.audits.update((existing) => [...existing, ...audits]);
          this.hasMoreAudits.set(audits.length === this.pageSize());
          this.loading.set(false);
          this.page.update((p) => p + 1);
        });
    });
  }

  onScroll(event: any): void {
    var viewport = event as CdkVirtualScrollViewport;
    const end = viewport.getRenderedRange().end;
    const total = viewport.getDataLength();
    if (end === total && this.hasMoreAudits() && !this.loading()) {
      this.scrollTrigger.next();
    }
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
