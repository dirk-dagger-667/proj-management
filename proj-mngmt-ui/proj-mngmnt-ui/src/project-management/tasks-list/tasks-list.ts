import { Component, effect, input, output, signal } from '@angular/core';
import { TaskListItem } from '../../client/models';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { StatusLabels } from '../../shared/status.lables';
import { MatFormField } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-tasks-list',
  imports: [
    ReactiveFormsModule,
    MatListModule,
    MatIconModule,
    MatFormField,
    FormsModule,
    MatInputModule,
  ],
  templateUrl: './tasks-list.html',
  styleUrl: './tasks-list.scss',
})
export class TasksList {
  public tasksInput = input<TaskListItem[]>();
  public searchTermInput = input<string>();
  public taskIdOutput = output<string>();
  public searchTermOutput = output<string>();
  public statusLabels = StatusLabels;

  public searchControl = new FormControl('');

  constructor() {
    effect(() => {
      if (this.searchTermInput() !== this.searchControl.value) {
        this.searchControl.setValue(this.searchTermInput() || null, { emitEvent: false });
      }
    });
  }
  openTask(taskId: string) {
    this.taskIdOutput.emit(taskId);
  }

  onInput(searchTerm: string) {
    this.searchTermOutput.emit(searchTerm);
  }
}
