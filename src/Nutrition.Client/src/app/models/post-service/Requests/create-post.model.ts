export interface CreatePostModel {
  title: string;
  text: string;
  keyWords: string[];
  ownerEmail: string;
  ownerId: string;
  files: File[];
}
