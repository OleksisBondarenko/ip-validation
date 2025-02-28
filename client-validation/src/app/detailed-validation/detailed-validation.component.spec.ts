import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailedValidationComponent } from './detailed-validation.component';

describe('DetailedValidationComponent', () => {
  let component: DetailedValidationComponent;
  let fixture: ComponentFixture<DetailedValidationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailedValidationComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DetailedValidationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
