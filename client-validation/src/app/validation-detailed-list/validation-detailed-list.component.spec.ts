import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ValidationDetailedListComponent } from './validation-detailed-list.component';

describe('ValidationDetailedListComponent', () => {
  let component: ValidationDetailedListComponent;
  let fixture: ComponentFixture<ValidationDetailedListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ValidationDetailedListComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ValidationDetailedListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
