import { Injectable } from '@angular/core';

import { BaseApiService } from './basiApi.service';
import { Category, CreateCategoryRequest } from '../models/categoryModel';

@Injectable()
export class CategoryService extends BaseApiService<Category, CreateCategoryRequest> {
  protected endpoint = 'Category';
}