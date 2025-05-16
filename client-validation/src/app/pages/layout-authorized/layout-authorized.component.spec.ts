import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LayoutAuthorizedComponent } from './layout-authorized.component';

describe('LayoutAuthorizedComponent', () => {
  let component: LayoutAuthorizedComponent;
  let fixture: ComponentFixture<LayoutAuthorizedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LayoutAuthorizedComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LayoutAuthorizedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
