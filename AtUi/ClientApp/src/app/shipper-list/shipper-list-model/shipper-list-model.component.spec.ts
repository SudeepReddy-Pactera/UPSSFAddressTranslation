import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ShipperListModelComponent } from './shipper-list-model.component';

describe('ShipperListModelComponent', () => {
  let component: ShipperListModelComponent;
  let fixture: ComponentFixture<ShipperListModelComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ShipperListModelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShipperListModelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
