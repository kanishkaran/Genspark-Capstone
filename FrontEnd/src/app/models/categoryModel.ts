export interface Category {
  id: string;
  categoryName: string;
  access: string;
}

export interface CreateCategoryRequest {
  categoryName: string;
  accessLevel: string;
}