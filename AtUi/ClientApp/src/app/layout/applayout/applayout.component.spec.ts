import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ApplayoutComponent } from './applayout.component';

describe('ApplayoutComponent', () => {
  let component: ApplayoutComponent;
  let fixture: ComponentFixture<ApplayoutComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplayoutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
