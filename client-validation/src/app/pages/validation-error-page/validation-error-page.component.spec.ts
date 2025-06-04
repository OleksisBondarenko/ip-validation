import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ValidationErrorPageComponent } from './validation-error-page.component';

describe('ValidationErrorPageComponent', () => {
  let component: ValidationErrorPageComponent;
  let fixture: ComponentFixture<ValidationErrorPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ValidationErrorPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ValidationErrorPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
