import { Component, input, output } from '@angular/core';
import { Status, TaskListItem, TasksService } from '../../client';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { StatusLabels } from '../../shared/status.lables';

@Component({
  selector: 'app-projects-board',
  imports: [CommonModule, MatCardModule, DragDropModule],
  templateUrl: './projects-board.html',
  styleUrl: './projects-board.scss',
})
export class ProjectsBoardComponent {
  public tasksInput = input<TaskListItem[]>();
  public statusesInput = Object.values(Status).filter((value) => typeof value === 'number');
  public taskIdOutput = output<string>();
  public statusLabels = StatusLabels;
  public statusChangedOutput = output<{ taskId: string; newStaus: Status }>();

  getTasksByStatus(status: Status) {
    return this.tasksInput()?.filter((task) => task.status === status);
  }

  emitTaskId(taskId: string) {
    this.taskIdOutput.emit(taskId);
  }
}
