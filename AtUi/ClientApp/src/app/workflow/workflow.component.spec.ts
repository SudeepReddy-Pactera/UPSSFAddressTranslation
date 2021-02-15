import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EmployeeComponent } from './workflow.component';

describe('CreateSourceComponent', () => {
    let component: EmployeeComponent;
    let fixture: ComponentFixture<EmployeeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
        declarations: [EmployeeComponent]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(EmployeeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
