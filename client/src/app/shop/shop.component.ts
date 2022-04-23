import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IProduct } from '../shared/models/product';
import { IProductBrand } from '../shared/models/ProductBrand';
import { IProductType } from '../shared/models/ProductType';
import { ShopParams } from '../shared/models/shopParams';
import { ShopService } from './shop.service';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  @ViewChild('search', {static: false}) searchTerm: ElementRef
  products: IProduct[];
  productBrands: IProductBrand[];
  productTypes: IProductType[];

  // Moved in one class
  // productBrandIdSelected = 0;
  // productTypeIdSelected = 0;
  // sortSelected = 'name';
  shopParams = new ShopParams()
  totalCount: number;
  sortOptions = [
    {name: 'Alphabetical', value: 'name'},
    {name: 'Price: Low to High', value: 'priceAsc'},
    {name: 'Price: High to Low', value: 'priceDesc'}
  ];

  constructor(private shopService: ShopService) { }


  ngOnInit(): void {
    this.getProducts();
    this.getProductBrands();
    this.getProductTypes();
  }

  getProducts() {
    // this.shopService.getProducts().subscribe(response => {
    //   this.products = response.data;
    // }, error => {
    //   console.log(error);
    // })

    // this.shopService.getProducts(this.productBrandIdSelected, this.productTypeIdSelected, this.sortSelected).subscribe(response => {
    //   this.products = response.data;
    // }, error => {
    //   console.log(error);
    // })

    // Using ShopParams
    this.shopService.getProducts(this.shopParams).subscribe(response => {
        this.products = response.data;
        this.shopParams.pageNumber = response.pageIndex
        this.shopParams.pageSize = response.pageSize;
        this.totalCount = response.count;
      }, error => {
        console.log(error);
      })
  }

  getProductBrands() {
    this.shopService.getProductBrands().subscribe(response => {
      this.productBrands = [{id: 0, name: 'All'}, ...response]; //For default reset filters
    }, error => {
      console.log(error);
    });
  }

  getProductTypes() {
    this.shopService.getProductTypes().subscribe(response => {
      this.productTypes = [{id: 0, name: 'All'}, ...response]; //For default reset filters
    }, error => {
      console.log(error);
    });
  }

  onProductBrandSelected(productBrandId: number) {
    this.shopParams.productBrandIdSelected = productBrandId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onProductTypeSelected(productTypeId: number) {
    this.shopParams.productTypeIdSelected = productTypeId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onSortSelected(sort: string) {
    this.shopParams.sortSelected = sort;
    this.getProducts();
  }

  onPageChanged(event: any) {
    if (this.shopParams.pageNumber !== event) {
    // this.shopParams.pageNumber = event.page;
    this.shopParams.pageNumber = event;
    this.getProducts();
    }
  }

  onSearch() {
    this.shopParams.productSearch = this.searchTerm.nativeElement.value;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onReset() {
    this.searchTerm.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.getProducts();
  }

}
