import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LayoutAnonymousComponent } from './layout-anonymous.component';

describe('LayoutAnonymousComponent', () => {
  let component: LayoutAnonymousComponent;
  let fixture: ComponentFixture<LayoutAnonymousComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LayoutAnonymousComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LayoutAnonymousComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
