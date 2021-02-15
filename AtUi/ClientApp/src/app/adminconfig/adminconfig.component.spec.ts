import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AdminconfigComponent } from './adminconfig.component';

describe('AdminconfigComponent', () => {
  let component: AdminconfigComponent;
  let fixture: ComponentFixture<AdminconfigComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AdminconfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminconfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
