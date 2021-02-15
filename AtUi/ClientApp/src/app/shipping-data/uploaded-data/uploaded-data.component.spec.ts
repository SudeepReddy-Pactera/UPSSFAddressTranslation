import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { UploadedDataComponent } from './uploaded-data.component';

describe('UploadedDataComponent', () => {
  let component: UploadedDataComponent;
  let fixture: ComponentFixture<UploadedDataComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadedDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadedDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
