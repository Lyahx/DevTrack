export type TagResponse = {
  id: number;
  userId: number;
  name: string;
  color: string | null;
  createdAt: string;
  updatedAt: string | null;
};

export type TagCreateRequest = {
  name: string;
  color?: string | null;
};

export type TagUpdateRequest = TagCreateRequest;
