import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { IPagination, Pagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';
import { IProductBrand } from '../shared/models/ProductBrand';
import { IProductType } from '../shared/models/ProductType';
import { ShopParams } from '../shared/models/shopParams';
@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://localhost:5001/api/';
  products: IProduct[] = [];
  brands: IProductBrand[] = [];
  types: IProductType[] = [];
  pagination = new Pagination();
  shopParams = new ShopParams();
  productCache = new Map();

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
  // getProducts(shopParams: ShopParams) {
  //   let params = new HttpParams();

  //   if (shopParams.productBrandIdSelected !== 0) {
  //     params = params.append('brandId', shopParams.productBrandIdSelected.toString());
  //   }

  //   if (shopParams.productTypeIdSelected !== 0) {
  //     params = params.append('typeId', shopParams.productTypeIdSelected.toString());
  //   }

  //   // if(shopParams.sortSelected) {
  //   //   params = params.append('sort', shopParams.sortSelected);
  //   // }

  //   params = params.append('sort', shopParams.sortSelected);
  //   params = params.append('pageIndex', shopParams.pageNumber.toString());
  //   params = params.append('pageSize', shopParams.pageSize.toString());

  //   if (shopParams.productSearch) {
  //     params = params.append('search', shopParams.productSearch)
  //   }

  //   return this.http.get<IPagination>(this.baseUrl + 'products', {observe: 'response', params})
  //     .pipe( //Use pipe if returning data from body
  //       map(response => {

  //         // Using cache
  //         this.products = response.body.data;

  //         return response.body;
  //       })
  //     );
  // }

  // Caching
  getProducts(useCache: boolean) {
    if (useCache === false) {
      this.productCache = new Map();
    }

    if (this.productCache.size > 0 && useCache === true) {
      if (this.productCache.has(Object.values(this.shopParams).join('-'))) {
        this.pagination.data = this.productCache.get(Object.values(this.shopParams).join('-'));
        return of(this.pagination);
      }
    }


    let params = new HttpParams();

    if (this.shopParams.productBrandIdSelected !== 0) {
      params = params.append('brandId', this.shopParams.productBrandIdSelected.toString());
    }

    if (this.shopParams.productTypeIdSelected !== 0) {
      params = params.append('typeId', this.shopParams.productTypeIdSelected.toString());
    }

    // if(shopParams.sortSelected) {
    //   params = params.append('sort', shopParams.sortSelected);
    // }

    params = params.append('sort', this.shopParams.sortSelected);
    params = params.append('pageIndex', this.shopParams.pageNumber.toString());
    params = params.append('pageSize', this.shopParams.pageSize.toString());

    if (this.shopParams.productSearch) {
      params = params.append('search', this.shopParams.productSearch)
    }

    return this.http.get<IPagination>(this.baseUrl + 'products', {observe: 'response', params})
      .pipe( //Use pipe if returning data from body
        map(response => {

          // Using cache
          //this.products = response.body.data;

          // Caching
          this.productCache.set(Object.values(this.shopParams).join('-'), response.body.data);
          this.pagination = response.body;
          return this.pagination;

          // return response.body;
        })
      );
  }

  setShopParams(params: ShopParams) {
    this.shopParams = params;
  }

  getShopParams() {
    return this.shopParams;
  }

  getProductBrands(){

    // Using cache
    if (this.brands.length > 0) {
      return of(this.brands);
    }

    return this.http.get<IProductBrand[]>(this.baseUrl + 'products/brands').pipe(
      map(response => {
        this.brands = response;
        return response;
      })
    );
  }

  getProductTypes() {

    // Using cache
    if (this.types.length > 0) {
      return of(this.types);
    }

    return this.http.get<IProductType[]>(this.baseUrl + 'products/types').pipe(
      map(response => {
        this.types = response;
        return response;
      })
    );
  }

  getProduct(id: number) {
    // Using cache
    // const product = this.products.find(p => p.id === id);
    let product: IProduct;
    this.productCache.forEach((products: IProduct[]) => {
      product = products.find(p => p.id === id);
    })

    if (product) {
      return of(product);
    }

    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }

}
