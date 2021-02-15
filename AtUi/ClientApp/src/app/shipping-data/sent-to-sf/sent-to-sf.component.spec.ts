import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SentToSfComponent } from './sent-to-sf.component';

describe('SentToSfComponent', () => {
  let component: SentToSfComponent;
  let fixture: ComponentFixture<SentToSfComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SentToSfComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SentToSfComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
