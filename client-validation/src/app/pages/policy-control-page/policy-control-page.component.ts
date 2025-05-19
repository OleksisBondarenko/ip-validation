import {Component, OnInit, ViewChild} from '@angular/core';
import {PolicyService} from "../../services/policy.service";
import {tap} from "rxjs";
import {AccessAction, AccessPolicyModel} from "../../models/accessPolicy.model";
import {MatGridList, MatGridTile} from "@angular/material/grid-list";
import {MatDivider} from "@angular/material/divider";
import {
  MatList,
  MatListItem,
  MatListItemIcon,
  MatListItemLine,
  MatListSubheaderCssMatStyler
} from "@angular/material/list";
import {MatIcon} from "@angular/material/icon";
import {FormsModule} from "@angular/forms";
import {MatButton, MatFabButton, MatIconButton, MatMiniFabButton} from "@angular/material/button";
import {DataSource} from "@angular/cdk/collections";
import {
  MatCell, MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable,
  MatTableDataSource
} from "@angular/material/table";
import AuditRecordModel from "../../models/auditData.model";
import {MatPaginator} from "@angular/material/paginator";
import {DatePipe} from "@angular/common";
import {MatStepper} from "@angular/material/stepper";

const emptyPolicy: AccessPolicyModel = {
  name: "",
  description: "",
  action: AccessAction.allow,
  ipFilterRules: [],
  order: 0,
  isActive: true,
  id: 0,
  createdOn: new Date(),
  policyEndDateTime: new Date(),
  policyStartDateTime: new Date(),
}
@Component({
  selector: 'app-policy-control-page',
  standalone: true,
  imports: [
    MatIcon,
    MatListItemIcon,
    FormsModule,
    MatTable,
    MatColumnDef,
    MatHeaderCell,
    MatCell,
    MatHeaderRow,
    MatRow,
    MatHeaderCellDef,
    MatCellDef,
    MatHeaderRowDef,
    MatRowDef,
    MatPaginator,
    DatePipe,
    MatIconButton,
    MatButton,
    MatStepper
  ],
  templateUrl: './policy-control-page.component.html',
  styleUrl: './policy-control-page.component.scss'
})
export class PolicyControlPageComponent implements OnInit {
    displayedColumns: string[] = [
      'name',
      // 'description',
      'isActive',
      // 'order',
      'ipFilterRules',
      'action',
      'createdOn',
      // 'policyStartDateTime',
      // 'policyEndDateTime',
      'changeOrder',
      'tableAction'
    ];
    dataSource: MatTableDataSource<AccessPolicyModel> = null!;
    @ViewChild(MatPaginator) paginator: MatPaginator = null!;

    pageSizes = [5, 10, 20];
    isLoading = false;
    totalData = 0;

    selectedPolicy: AccessPolicyModel = emptyPolicy;
    newPolicy: AccessPolicyModel = emptyPolicy;

    ipRulesString: string = '';
    newPolicyIpRulesString: string = '';

    constructor( private policyService: PolicyService) {
      this.dataSource = new MatTableDataSource<AccessPolicyModel>([]);
    }

    orderPolicies  (unOrderedPolicies: AccessPolicyModel []):  AccessPolicyModel[] {
      const ordered = unOrderedPolicies.sort((policyA, policyB ) => policyA.order - policyB.order);

      return ordered;
    }

    ngOnInit() {
      this.fetchAllPolicies();
    }

  setSelectedPolicy(policy: AccessPolicyModel) {
    this.selectedPolicy = { ...policy }; // clone to avoid direct binding if needed
    this.ipRulesString = policy.ipFilterRules.join(', ');
  }

  onIpRulesChange(value: string) {
    if (this.selectedPolicy) {
      this.selectedPolicy.ipFilterRules = value.split(',').map(ip => ip.trim());
    }
  }

  updatePolicy() {
    if (!confirm(`Ви впевнені, що хочете оновити політику "${this.selectedPolicy.name}"?`)) return;

    this.policyService.updatePolicy(this.selectedPolicy.id, this.selectedPolicy)
      .subscribe(() => {
        this.fetchAllPolicies();
        alert("Політика оновлена успішно!");
      });
  }

    fetchAllPolicies () {
      this.isLoading = true;

      this.policyService.getAllPolicies()
        .pipe(
          tap((policies) => {
            this.totalData = policies.length;
            const ordered = this.orderPolicies(policies);
            this.dataSource = new MatTableDataSource<AccessPolicyModel>(ordered);
          })
        )
        .subscribe()

      this.isLoading = false;
    }

  createPolicy() {
    const policyToCreate = {
      ...this.newPolicy,
      ipFilterRules: this.newPolicyIpRulesString.split(',').map(ip => ip.trim())
    };

    this.policyService.createPolicy(policyToCreate)
      .subscribe((created) => {
        this.fetchAllPolicies();
        this.newPolicy = { ...emptyPolicy };
        this.newPolicyIpRulesString = '';
        alert("Політика створена успішно!");
      });
  }

  reorderPolicies(policy: AccessPolicyModel, isUp: boolean) {
      // debugger
    // if (!confirm(`Ви впевнені, що хочете змінити порядок політик "${policy.name}"?`)) return;

    this.policyService.reorderPolicies(policy.id, isUp).subscribe(() => {
      this.fetchAllPolicies();
      alert("Порядок застосування політик змінено!");
    });
  }
  deletePolicy(policy: AccessPolicyModel) {
    if (!confirm(`Ви впевнені, що хочете видалити політику "${policy.name}"?`)) return;

    this.policyService.deletePolicy(policy.id).subscribe(() => {
      this.fetchAllPolicies();
      alert("Політика видалена!");
    });
  }


  protected readonly AccessAction = AccessAction;
  protected readonly Number = Number;
}
