import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { IPagination } from '../shared/models/pagination';
import { IProductBrand } from '../shared/models/ProductBrand';
import { IProductType } from '../shared/models/ProductType';
import { ShopParams } from '../shared/models/shopParams';
@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http: HttpClient) { }

  // getProducts(productBrandId?: number, productTypeId?: number, sort?: string) {
  //   let params = new HttpParams();

  //   if (productBrandId) {
  //     params = params.append('brandId', productBrandId.toString());
  //   }

  //   if (productTypeId) {
  //     params = params.append('typeId', productTypeId.toString());
  //   }

  //   if(sort) {
  //     params = params.append('sort', sort);
  //   }

  //   return this.http.get<IPagination>(this.baseUrl + 'products', {observe: 'response', params})
  //     .pipe( //Use pipe if returning data from body
  //       map(response => {
  //         return response.body;
  //       })
  //     );
  // }

  // Using ShopParams
  getProducts(shopParams: ShopParams) {
    let params = new HttpParams();

    if (shopParams.productBrandIdSelected !== 0) {
      params = params.append('brandId', shopParams.productBrandIdSelected.toString());
    }

    if (shopParams.productTypeIdSelected !== 0) {
      params = params.append('typeId', shopParams.productTypeIdSelected.toString());
    }

    // if(shopParams.sortSelected) {
    //   params = params.append('sort', shopParams.sortSelected);
    // }

    params = params.append('sort', shopParams.sortSelected);
    params = params.append('pageIndex', shopParams.pageNumber.toString());
    params = params.append('pageSize', shopParams.pageSize.toString());

    if (shopParams.productSearch) {
      params = params.append('search', shopParams.productSearch)
    }

    return this.http.get<IPagination>(this.baseUrl + 'products', {observe: 'response', params})
      .pipe( //Use pipe if returning data from body
        map(response => {
          return response.body;
        })
      );
  }

  getProductBrands(){
    return this.http.get<IProductBrand[]>(this.baseUrl + 'products/brands')
  }

  getProductTypes() {
    return this.http.get<IProductType[]>(this.baseUrl + 'products/types')
  }

}
