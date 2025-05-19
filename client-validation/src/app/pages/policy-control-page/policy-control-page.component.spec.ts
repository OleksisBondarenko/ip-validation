import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PolicyControlPageComponent } from './policy-control-page.component';

describe('PolicyControlPageComponent', () => {
  let component: PolicyControlPageComponent;
  let fixture: ComponentFixture<PolicyControlPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PolicyControlPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PolicyControlPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
