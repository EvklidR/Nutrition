export interface CreatePostModel {
  title: string;
  text: string;
  keyWords: string[];
  ownerEmail: string | null;
  ownerId: string | null;
  files: File[];
}
