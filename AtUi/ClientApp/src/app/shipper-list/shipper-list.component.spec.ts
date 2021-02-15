import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ShipperListComponent } from './shipper-list.component';

describe('ShipperListComponent', () => {
  let component: ShipperListComponent;
  let fixture: ComponentFixture<ShipperListComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ShipperListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShipperListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
