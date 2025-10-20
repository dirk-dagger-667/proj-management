import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TasksPreview } from './tasks-preview';

describe('TasksPreview', () => {
  let component: TasksPreview;
  let fixture: ComponentFixture<TasksPreview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TasksPreview]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TasksPreview);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
