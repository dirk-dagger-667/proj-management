import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectsBoard } from './projects-board';

describe('ProjectsBoard', () => {
  let component: ProjectsBoard;
  let fixture: ComponentFixture<ProjectsBoard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectsBoard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectsBoard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
