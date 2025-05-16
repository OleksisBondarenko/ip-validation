import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WhiteListPageComponent } from './white-list-page.component';

describe('WhiteListPageComponent', () => {
  let component: WhiteListPageComponent;
  let fixture: ComponentFixture<WhiteListPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WhiteListPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WhiteListPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
