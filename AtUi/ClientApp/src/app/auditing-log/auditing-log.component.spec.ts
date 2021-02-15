import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AuditingLogComponent } from './auditing-log.component';

describe('AuditingLogComponent', () => {
  let component: AuditingLogComponent;
  let fixture: ComponentFixture<AuditingLogComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AuditingLogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AuditingLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
