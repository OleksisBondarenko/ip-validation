import {Component, OnInit} from '@angular/core';
import AuditDataModel from "../models/auditDataModel";
import {ValidationAuditDataService} from "../validation-audit-data.service";
@Component({
  selector: 'app-validation-detailed-list',
  standalone: true,
  imports: [],
  templateUrl: './validation-detailed-list.component.html',
  styleUrl: './validation-detailed-list.component.scss'
})
export class ValidationDetailedListComponent implements OnInit {
  dataModels: AuditDataModel[] = [];  // Array to hold the fetched heroes
  loading: boolean = true;         // Flag to show loading spinner if needed
  error: string | null = null;     // Error handling (optional)

  constructor(private validationDataService: ValidationAuditDataService ) {
  }

  ngOnInit(): void {
    // Call the getHeroes method and subscribe to the Observable
    this.validationDataService.getData().subscribe(
      (data) => {
        this.dataModels = data;  // Assign the fetched data to the heroes array
        this.loading = false; // Set loading to false once data is loaded
      },
      (error) => {
        this.error = 'Error loading heroes';  // Handle error (optional)
        this.loading = false;  // Stop loading indicator
      }
    );
  }
}
