import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LoginlayoutComponent } from './loginlayout.component';

describe('LoginlayoutComponent', () => {
  let component: LoginlayoutComponent;
  let fixture: ComponentFixture<LoginlayoutComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LoginlayoutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginlayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
