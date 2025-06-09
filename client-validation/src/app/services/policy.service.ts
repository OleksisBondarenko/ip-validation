import { Injectable } from '@angular/core';
import {ApiService} from "./api.service";
import {AccessPolicyModel} from "../models/accessPolicy.model";

@Injectable({
  providedIn: 'root'
})
export class PolicyService {

  constructor(private apiService: ApiService) {

  }

  getAllPolicies() {
    return this.apiService.get<AccessPolicyModel []>("api/v1/AccessPolicy");
  }

  getAllPolicyById(id: number) {
    return this.apiService.get<AccessPolicyModel>(`api/v1/AccessPolicy/${id}`);
  }

  updatePolicy(id: number, newPolicyDto: AccessPolicyModel) {
    return this.apiService.put<AccessPolicyModel>(`api/v1/AccessPolicy/${id}`, newPolicyDto);
  }

  createPolicy(newPolicyDto: AccessPolicyModel) {
    return this.apiService.post<AccessPolicyModel>(`api/v1/AccessPolicy`, newPolicyDto);
  }

  reorderPolicies(id: number, isUp: boolean) {
    return this.apiService.patch<AccessPolicyModel>(`api/v1/AccessPolicy/reorder`, {
      policyId: id,
      isUp: isUp
    });
  }


  deletePolicy(id: number) {
    return this.apiService.delete<AccessPolicyModel>(`api/v1/AccessPolicy/${id}`);
  }


}
